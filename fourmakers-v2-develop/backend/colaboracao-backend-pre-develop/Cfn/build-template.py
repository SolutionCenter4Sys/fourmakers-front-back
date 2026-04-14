#!/usr/bin/env python3
import os
import re
from dotenv import load_dotenv
import argparse
import yaml
import subprocess
from pathlib import Path
import json

load_dotenv()
projects_path = "projects.json"
parser = argparse.ArgumentParser(description="Generate CloudFormation YAML", formatter_class=argparse.ArgumentDefaultsHelpFormatter)
parser.add_argument('branch', help='branch name')
args = parser.parse_args()
config = vars(args)
#config = {}
#config['branch'] = 'develop'

config['template_task'] = 'ecs-task.tpl'
config['template_service'] = 'ecs-service.tpl'
config['template_rule'] = 'alb-listener-rule.tpl'

ok = True

if not (os.path.isfile(config['template_task'])):
    print('file not found: %s' % config['template_task'])
    ok = False

if not (os.path.isfile(config['template_service'])):
    print('file not found: %s' % config['template_service'])
    ok = False

ref = dict([
    ('develop','dev'),
    ('main','prd')])

AWS_ORG = os.environ.get('AWS_ORG', "")

def profile(env : str):
    return AWS_ORG+'-production' if env=='prd' else AWS_ORG+'-development'

def bucket_ref(env: str):
    return 'prd' if env=='prd' else 'dev'

def postfix(env : str):
    return '' if env=='prd' else '-'+env

def task_template_url(env :str):
    return 'https://'+AWS_ORG+'-bucket-cfn-'+bucket_ref(env)+'.s3.amazonaws.com/ecs-task-prd-template.yaml' if env=='prd' else 'https://'+AWS_ORG+'-bucket-cfn-'+bucket_ref(env)+'.s3.amazonaws.com/ecs-task-template.yaml'

def service_template_url(env :str):
    return 'https://'+AWS_ORG+'-bucket-cfn-'+bucket_ref(env)+'.s3.amazonaws.com/ecs-service-template.yaml'

def alb_rule_template_url(env :str):
    return 'https://'+AWS_ORG+'-bucket-cfn-'+bucket_ref(env)+'.s3.amazonaws.com/alb-listener-rule-template.yaml'

def ecs_cluster(env :str):
    return 'ecs' if env=='prd' else 'ecs'

def proxy_path(env :str):
    return 'api' if env=='prd' else 'fourmakershub-api'

def env_file_path(env :str):
    return '.env-prd' if env=='prd' else '.env'

def db_database(env :str):
    return DB_NAME_PRD if env=='prd' else DB_NAME_DEV

def server_name(env :str):
    return DB_HOST_PRD if env=='prd' else DB_HOST_DEV

if not (config['branch'] in list(ref.keys())):
    print('invalid branch name: %s' % config['branch'])
    print('valid names: %s' % list(ref.keys()))
    ok = False

if not ok:
    exit(-1)

config['env'] = ref[config['branch']]

try:
    with open(config['template_task'], encoding="utf-8") as yaml_file:
        cf1 = yaml.full_load(yaml_file)
except:
    print('YAML file template %s error.' % config['template_task'])
    exit(-1)

try:
    with open(config['template_service'], encoding="utf-8") as yaml_file:
        cf2 = yaml.full_load(yaml_file)
except:
    print('YAML file %s error.' % config['template_service'])
    exit(-1)

try:
    with open(config['template_rule'], encoding="utf-8") as yaml_file:
        cfRule = yaml.full_load(yaml_file)
except:
    print('YAML file %s error.' % config['template_service'])
    exit(-1)

try:
    cmd = "aws elbv2 describe-listeners --load-balancer $(aws elbv2 describe-load-balancers|jq -r '.LoadBalancers[]|select(.Type == \"application\")|select(.LoadBalancerName|test(\".-"+config['env']+"\"))|.LoadBalancerArn')|jq -r '.Listeners[]|select(.Protocol==\"HTTPS\")|.ListenerArn'"
    status, output = subprocess.getstatusoutput(cmd)
    if not (status == 0):
        print('Could not obtain Listener Arn from AWS::ElasticLoadBalancingV2::Listener')
        exit(-1)
    LISTENER_ARN = output
except:
    print('Could not obtain Listener Arn from AWS::ElasticLoadBalancingV2::Listener')
    exit(-1)

try:
    cmd = "aws elbv2 describe-rules --listener-arn "+LISTENER_ARN+"|jq -r '.Rules[]|select(.IsDefault==false)|.Priority'| jq -r --slurp '.|max'|xargs echo '1 +'|bc"
    status, output = subprocess.getstatusoutput(cmd)
    if not (status == 0):
        print('Could not obtain Priority number from AWS::ElasticLoadBalancingV2::ListenerRule')
        exit(-1)
    PRIORITY = int(output)
except:
    print('Could not obtain Priority number from AWS::ElasticLoadBalancingV2::ListenerRule')
    exit(-1)

try:
    cmd = "aws secretsmanager list-secrets | jq -r '.SecretList[].ARN' | grep --color=never '_" + config['env'] + "-'"
    status, output = subprocess.getstatusoutput(cmd)
    if not (status == 0):
        print('Could not obtain Secrets Manager entry ARN from AWS')
        exit(-1)
    SM_ARN = output
except:
    print('Could not obtain Secrets Manager entry ARN from AWS')
    exit(-1)

try:
    cmd = "git config --local remote.origin.url"
    status, output = subprocess.getstatusoutput(cmd)
    if not (status == 0):
        #print('Could not obtain Git Config')
        #exit(-1)
        url = os.environ.get('GIT_URL', "")
    else:
        url = output
except:
    #print('Could not obtain Git Config')
    #exit(-1)
    url = os.environ.get('GIT_URL', "")


with open(projects_path, "r") as projects_file:
    projects_data = json.load(projects_file)
    for indice, project_info in enumerate(projects_data):
        PRIORITY = PRIORITY + 1
        project_path = '../ColaboracaoBackend/' + project_info["projectPath"].strip() + '/Cfn/';
        dotenv_path = Path(project_path + '.env')
        load_dotenv(dotenv_path=dotenv_path,override=True)

        matches = re.search(r".*\.com[\/\:](.*)\/(.*)\/(.*)\.git", url)
        HEALTH_CHECK_PATH = os.environ.get('HEALTH_CHECK_PATH', "/")
        REPOSITORY_GROUP = matches.group(2)
        PROJECT = os.environ.get('PROJECT', matches.group(3))
        HOSTNAME_BASE = os.environ.get('HOST_NAME_BASE', PROJECT)

        DB_NAME_DEV = os.environ.get('DB_NAME_DEV')
        DB_NAME_PRD = os.environ.get('DB_NAME_PRD')
        DB_HOST_DEV = os.environ.get('DB_HOST_DEV')
        DB_HOST_PRD = os.environ.get('DB_HOST_PRD')

        cf1['Parameters']['AwsProfile']['AllowedValues'][0] = profile(config['env'])
        cf1['Parameters']['RepositoryGroup']['Default'] = REPOSITORY_GROUP
        cf1['Parameters']['Project']['Default'] = PROJECT
        cf1['Parameters']['Env']['Default'] = config['env']
        #cf1['Parameters']['HostName']['Default'] = HOSTNAME_BASE+postfix(config['env'])
        cf1['Parameters']['HostName']['Default'] = HOSTNAME_BASE
        cf1['Parameters']['Secrets']['Default'] = SM_ARN
        cf1['Parameters']['HealthCheckPath']['Default'] = HEALTH_CHECK_PATH
        cf1['Parameters']['Priority']['Default'] = PRIORITY
        cf1['Resources']['Stack']['Properties']['TemplateURL'] = task_template_url(config['env'])

        cf2['Parameters']['AwsProfile']['AllowedValues'][0] = profile(config['env'])
        cf2['Parameters']['Project']['Default'] = PROJECT
        cf2['Parameters']['Env']['Default'] = config['env']
        cf2['Resources']['Stack']['Properties']['TemplateURL'] = service_template_url(config['env'])

        subprocess.run(['mkdir', PROJECT])

        try:
            with open(PROJECT + '/' + PROJECT+'-task-'+config['env']+'.yaml', 'w', encoding="utf-8") as yaml_file_output:
                yaml.dump(cf1, yaml_file_output)
        except:
            print('error saving YAML file %s' % PROJECT+'-task-'+config['env']+'.yaml')
            exit(-1)
        print('Indice atual: %s' % indice)
        try:
            with open(PROJECT + '/' + PROJECT+'-task-'+config['env']+'.env', 'w', encoding="utf-8") as env_file_output:
                common_env = env_file_path(config['env'])
                
                project_env = project_path + '.env'
                print('Teste: %s' % project_env)
                with open(common_env, "r") as common_env_read:
                    common_env_content = common_env_read.read()
                    print('Teste 2: %s' % common_env_content)

                with open(project_env, "r") as project_env_read:
                    project_env_content = project_env_read.read()
                    print('Teste 3: %s' % project_env_content)
                
                env_content = re.sub(r"(.*)\"{1}(.*)\"{1}", r"\1\2", '# General env variables:\n' + common_env_content + '\n\n# Project env variables:\n' + project_env_content + '\n')
                env_file_output.write(env_content)
        except Exception as e:
            print('error creating ENV file %s' % (PROJECT+'-task-'+config['env']+'.env'))
            print('error: %s' % e)
            exit(-1)

        try:
            with open(PROJECT + '/' + PROJECT+'-service-'+config['env']+'.yaml', 'w', encoding="utf-8") as yaml_file_output:
                yaml.dump(cf2, yaml_file_output)
        except:
            print('error saving YAML file %s' % PROJECT+'-service-'+config['env']+'.yaml')
            exit(-1)

        try:
            with open(PROJECT + '/' + PROJECT+'-scale-'+config['env']+'.sh', 'w', encoding="utf-8") as sh_file_output:
                sh_file_output.write('aws ecs update-service --cluster '+ecs_cluster(config['env'])+' --service srv-'+PROJECT+'-'+config['env']+' --desired-count $1\n')
            subprocess.run(['chmod','+x',PROJECT + '/' + PROJECT+'-scale-'+config['env']+'.sh'])
        except:
            print('error creating SH file %s' % PROJECT+'-scale-'+config['env']+'.sh')
            exit(-1)

        try:
            with open(PROJECT + '/' + PROJECT+'-redeploy-'+config['env']+'.sh', 'w', encoding="utf-8") as sh_file_output:
                sh_file_output.write('aws ecs update-service --cluster '+ecs_cluster(config['env'])+' --service srv-'+PROJECT+'-'+config['env']+' --enable-execute-command --force-new-deployment\n')
            subprocess.run(['chmod','+x',PROJECT + '/' + PROJECT+'-redeploy-'+config['env']+'.sh'])
        except:
            print('error creating SH file %s' % PROJECT+'-redeploy-'+config['env']+'.sh')
            exit(-1)

        try:
            with open(PROJECT + '/' + PROJECT+'-exec-'+config['env']+'.sh', 'w', encoding="utf-8") as sh_file_output:
                sh_file_output.write("# ecs update-service --cluster ecs --service srv-" + PROJECT + "-"+config['env']+" --enable-execute-command --force-new-deployment\nexport TASK=$(aws ecs list-tasks --cluster ecs --service-name srv-" + PROJECT + "-"+config['env']+"|jq -r '.taskArns[]'|head -n 1); aws ecs execute-command --cluster ecs --task  ${TASK} --interactive --command '/bin/sh'\n")
            subprocess.run(['chmod','+x',PROJECT + '/' + PROJECT+'-exec-'+config['env']+'.sh'])
        except:
            print('error creating SH file %s' % PROJECT+'-exec-'+config['env']+'.sh')
            exit(-1)
        
        for indice_controller, controller_info in enumerate(project_info["controllers"]):
            PRIORITY = PRIORITY + 1
            cfRule['Parameters']['Env']['Default'] = config['env']
            cfRule['Parameters']['HostName']['Default'] = project_info["serviceName"]
            cfRule['Parameters']['ProxyHostName']['Default'] = proxy_path(config['env'])
            cfRule['Parameters']['PathPattern']['Default'] = '/api/'+controller_info+'/*'
            cfRule['Parameters']['Priority']['Default'] = PRIORITY
            cfRule['Resources']['Stack']['Properties']['TemplateURL'] = alb_rule_template_url(config['env'])
            try:
                with open(PROJECT + '/' + controller_info+'-alb-listener-rule-'+config['env']+'.yaml', 'w', encoding="utf-8") as yaml_file_output:
                    yaml.dump(cfRule, yaml_file_output)
            except:
                print('error saving YAML file %s' % PROJECT+'-task-'+config['env']+'.yaml')
                exit(-1)
