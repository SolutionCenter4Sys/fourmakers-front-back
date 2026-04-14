AWSTemplateFormatVersion: 2010-09-09
Description: IaC Architectural Resources

Parameters:

  Env:
    Type: String
    Default: dev

  HostName:
    Type: String
    Default: service-api

  ProxyHostName:
    Type: String
    Default: fourmakershub-api
  
  PathPattern:
    Type: String
    Default: /api/path/*

  Priority:
    Type: String
    Default: 1

Resources:
  Stack:
    Properties:
      Parameters:
        StackName:
          Fn::Sub: ${AWS::StackName}
        Env:
          Fn::Sub: ${Env}
        HostName:
          Fn::Sub: ${HostName}
        ProxyHostName:
          Fn::Sub: ${ProxyHostName}
        PathPattern:
          Fn::Sub: ${PathPattern}
        Priority:
          Fn::Sub: ${Priority}
      TemplateURL: https://fsys2-bucket-cfn-dev.s3.amazonaws.com/alb-listener-rule-template.yaml
    Type: AWS::CloudFormation::Stack
