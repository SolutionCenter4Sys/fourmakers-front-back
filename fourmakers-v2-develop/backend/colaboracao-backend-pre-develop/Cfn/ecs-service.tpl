---

# ECS Service
# Mastria

AWSTemplateFormatVersion: 2010-09-09
Description: IaC Architectural Resources

Parameters:

  AwsProfile:
    Type: String
    AllowedValues:
    - fsys2-profile

  Project:
    Type: String
    Default: project

  Env:
    Type: String
    Default: env

Resources:

  Stack:
    Type: AWS::CloudFormation::Stack
    Properties:
      TemplateURL: https://fsys2-bucket-cfn-env.s3.amazonaws.com/ecs-service-template.yaml
      Parameters:
        StackName:
          Fn::Sub: ${AWS::StackName}
        AwsProfile:
          Fn::Sub: ${AwsProfile}
        Project:
          Fn::Sub: ${Project}
        Env:
          Fn::Sub: ${Env}
