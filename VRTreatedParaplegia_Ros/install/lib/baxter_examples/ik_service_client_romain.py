
import argparse
import struct
import sys

import rospy

import actionlib
from copy import copy
from geometry_msgs.msg import (
    PoseStamped,
    Pose,
    Point,
    Quaternion,
)
from std_msgs.msg import Header

from baxter_core_msgs.srv import (
    SolvePositionIK,
    SolvePositionIKRequest,
)

from control_msgs.msg import (
    FollowJointTrajectoryAction,
    FollowJointTrajectoryGoal,
)
from trajectory_msgs.msg import (
    JointTrajectoryPoint,
)

import baxter_interface

from baxter_interface import CHECK_VERSION

class Trajectory(object):
    def __init__(self, limb):
        ns = 'robot/limb/' + limb + '/'
        self._client = actionlib.SimpleActionClient(
            ns + "follow_joint_trajectory",
            FollowJointTrajectoryAction,
        )
        self._goal = FollowJointTrajectoryGoal()
        self._goal_time_tolerance = rospy.Time(0.1)
        self._goal.goal_time_tolerance = self._goal_time_tolerance
        server_up = self._client.wait_for_server(timeout=rospy.Duration(10.0))
        if not server_up:
            rospy.logerr("Timed out waiting for Joint Trajectory"
                         " Action Server to connect. Start the action server"
                         " before running example.")
            rospy.signal_shutdown("Timed out waiting for Action Server")
            sys.exit(1)
        self.clear(limb)

    def add_point(self, positions, time):
        point = JointTrajectoryPoint()
        point.positions = copy(positions)
        point.time_from_start = rospy.Duration(time)
        self._goal.trajectory.points.append(point)

    def start(self):
        self._goal.trajectory.header.stamp = rospy.Time.now()
        self._client.send_goal(self._goal)

    def stop(self):
        self._client.cancel_goal()

    def wait(self, timeout=15.0):
        self._client.wait_for_result(timeout=rospy.Duration(timeout))

    def result(self):
        return self._client.get_result()

    def clear(self, limb):
        self._goal = FollowJointTrajectoryGoal()
        self._goal.goal_time_tolerance = self._goal_time_tolerance
        self._goal.trajectory.joint_names = [limb + '_' + joint for joint in \
            ['s0', 's1', 'e0', 'e1', 'w0', 'w1', 'w2']]


def recup_points():
	f = open("fichier_points.txt","r")
	points = [line.split(',') for line in f]
	f.close()
	return points


def ik_test(limb,x,y,z):
    rospy.init_node("rsdk_joint_trajectory_client_%s" % (limb,))
    ns = "ExternalTools/" + limb + "/PositionKinematicsNode/IKService"
    iksvc = rospy.ServiceProxy(ns, SolvePositionIK)
    ikreq = SolvePositionIKRequest()
    hdr = Header(stamp=rospy.Time.now(), frame_id='base')
    print z,-x,y
    poses = {
        'left': PoseStamped(
            header=hdr,
            pose=Pose(
                position=Point(
                    x=z,
                    y=-x,
                    z=y,
                ),
                orientation=Quaternion(
                    x=1,#1000 face au sol 0000.5 face en lair
                    y=0,
                    z=0,
                    w=0,
                ),
            ),
        ),
        
    }

    ikreq.pose_stamp.append(poses[limb])
    try:
        rospy.wait_for_service(ns, 5.0)
        resp = iksvc(ikreq)
    except (rospy.ServiceException, rospy.ROSException), e:
        rospy.logerr("Service call failed: %s" % (e,))
        return 1

    # Check if result valid, and type of seed ultimately used to get solution
    # convert rospy's string representation of uint8[]'s to int's
    resp_seeds = struct.unpack('<%dB' % len(resp.result_type),
                               resp.result_type)
    if (resp_seeds[0] != resp.RESULT_INVALID):
        seed_str = {
                    ikreq.SEED_USER: 'User Provided Seed',
                    ikreq.SEED_CURRENT: 'Current Joint Angles',
                    ikreq.SEED_NS_MAP: 'Nullspace Setpoints',
                   }.get(resp_seeds[0], 'None')
        print("\nSUCCESS - Valid Joint Solution Found ")
        # Format solution into Limb API-compatible dictionary
        limb_joints = dict(zip(resp.joints[0].name, resp.joints[0].position))

        joints_pos.append(resp.joints[0].position)
    
    else:
        print("INVALID POSE - No Valid Joint Solution Found.")
        sys.exit()
    return 0


def main():
    arg_fmt = argparse.RawDescriptionHelpFormatter
    parser = argparse.ArgumentParser(formatter_class=arg_fmt,
                                     description=main.__doc__)
    limb = 'left'
    global joints_pos
    joints_pos=[]
    points=recup_points()
    for x in points:
    	ik_test(limb,float(x[0]),float(x[1]),float(x[2]))
    rs = baxter_interface.RobotEnable(CHECK_VERSION)
    rs.enable()
    print("\nRunning. Ctrl-c to quit\n")
   # positions = {
    #    'left': joints_pos,
   # }
    traj = Trajectory(limb)
    rospy.on_shutdown(traj.stop)
    # Command Current Joint Positions first
    limb_interface = baxter_interface.limb.Limb(limb)
    current_angles = [limb_interface.joint_angle(joint) for joint in limb_interface.joint_names()]
    traj.add_point(current_angles, 0.0)
    time=pas=15.0

    for line in joints_pos: 	
    	traj.add_point(line,time)
    	time += pas
    	
    print traj._goal
    traj.start()
    traj.wait(time)
    print("Exiting - Joint Trajectory Action Test Complete")
if __name__ == '__main__':
    sys.exit(main())

