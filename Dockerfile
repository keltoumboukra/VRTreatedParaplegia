FROM google/cloud-sdk:latest

CMD ["gsutil", "cp", "-r", ".", "gs://captures170219"]