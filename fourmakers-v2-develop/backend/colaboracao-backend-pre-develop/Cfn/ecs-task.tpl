---

# ECS Task
# Mastria

AWSTemplateFormatVersion: 2010-09-09
Description: IaC Architectural Resources

Parameters:

  AwsProfile:
    Type: String
    AllowedValues:
    - fsys2-profile

  RepositoryGroup:
    Type: String
    Default: fsys2/group

  Project:
    Type: String
    Default: project

  Env:
    Type: String
    Default: env

  HostName:
    Type: String
    Default: hostname

  Secrets:
    Type: String
    Default: secretsmanager

  HealthCheckPath:
    Type: String
    Default: /path

  Priority:
    Type: String
    Default: 1

Resources:

  Stack:
    Type: AWS::CloudFormation::Stack
    Properties:
      TemplateURL: https://fsys2-bucket-cfn-env.s3.amazonaws.com/ecs-task-template.yaml
      Parameters:
        StackName:
          Fn::Sub: ${AWS::StackName}
        AwsProfile:
          Fn::Sub: ${AwsProfile}
        RepositoryGroup:
          Fn::Sub: ${RepositoryGroup}
        Project:
          Fn::Sub: ${Project}
        Env:
          Fn::Sub: ${Env}
        HostName:
          Fn::Sub: ${HostName}
        Secrets:
          Fn::Sub: ${Secrets}
        HealthCheckPath:
          Fn::Sub: ${HealthCheckPath}
        Priority:
          Fn::Sub: ${Priority}
