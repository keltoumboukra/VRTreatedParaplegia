package com.autonomy.OculusBaxter.web.controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class PositionController {
    @RequestMapping(value="/Positions", method= RequestMethod.GET)
    public String PositionsList() {
        return "Here the list of positions";
    }
}

    @RequestMapping(value = "/Positions/{id}", method = RequestMethod.GET)
    public String printPositions(@PathVariable int id) {
        return id;
    }