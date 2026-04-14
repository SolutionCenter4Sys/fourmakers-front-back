# Makefile CloueFormation IaC
# Mastria

SHELL := /bin/bash

empty:=
space:= $(empty) $(empty)

ORG := ${AWS_ORG}

ID_PRODUCTION := ${AWS_ID_PRD}
ID_DEVELOPMENT := ${AWS_ID_DEV}

PROFILE_PRODUCTION := $(ORG)-production
PROFILE_DEVELOPMENT := $(ORG)-development

BUCKET_CFN_PRODUCTION := $(ORG)-bucket-cfn-prd
BUCKET_CFN_DEVELOPMENT := $(ORG)-bucket-cfn-dev

BUCKET_KEYS_PRODUCTION := $(ORG)-bucket-keys-prd
BUCKET_KEYS_DEVELOPMENT := $(ORG)-bucket-keys-dev

GREP := \
	$(shell \
	[ "$$(uname)" == "Linux" ] && echo "grep" && exit;\
	[ "$$(uname)" == "Darwin" ] && echo "egrep --color=never" && exit;)

SED := \
	$(shell \
	[ "$$(uname)" == "Linux" ] && echo "sed" && exit;\
	[ "$$(uname)" == "Darwin" ] && echo "gsed" && exit;)

BASE64 := \
	$(shell \
	[ "$$(uname)" == "Linux" ] && echo "base64" && exit;\
	[ "$$(uname)" == "Darwin" ] && echo "base64" && exit;)


SEQUENCE := \
	$(shell \
	[ -n "$${AWS_PROFILE}" ] && { [ "$${AWS_PROFILE}" == "$(PROFILE_PRODUCTION)" ] && echo PRODUCTION || echo DEVELOPMENT; } || echo UNDEFINED )

ALL := \
	$(shell \
	[ -f "$(SEQUENCE)" ] && { cat $(SEQUENCE); ls -1 *yaml|$(GREP) -v -f $(SEQUENCE) || true; } || ls -1 *yaml)

LINTALL := $(patsubst %,.lint-%.pass, $(ALL))

ALLSEQ := \
	$(shell \
	[ -f "$(SEQUENCE)" ] && cat $(SEQUENCE) || echo "" )

ALLSEQREGEX := $(subst $(space),|,$(patsubst %.yaml,%,$(ALLSEQ)))

ALLREV := \
	$(shell \
	echo $(ALLSEQ)|rev|tr -s ' ' '\0'|xargs -0 -n1 -I{} bash -c 'echo $$0|rev' {})

ALLREV := $(patsubst %.yaml,.clean-%, $(ALLREV))

TIME_STAMP := \
	$(shell \
	date "+%Y%m%d%H%M%S")

S3_BUCKET_CFN := \
	$(shell \
	[ -n "$${AWS_PROFILE}" ] && { [ "$${AWS_PROFILE}" == "$(PROFILE_PRODUCTION)" ] && \
	echo $(BUCKET_CFN_PRODUCTION) || \
	echo $(BUCKET_CFN_DEVELOPMENT); } || exit 1; )

S3_BUCKET_CFN_URL := \
	$(shell \
	[ -n "$${AWS_PROFILE}" ] && { [ "$${AWS_PROFILE}" == "$(PROFILE_PRODUCTION)" ] && \
	echo s3://$(BUCKET_CFN_PRODUCTION) || \
	echo s3://$(BUCKET_CFN_DEVELOPMENT); } || exit 1; )

S3_BUCKET_KEYS_URL := \
	$(shell \
	[ -n "$${AWS_PROFILE}" ] && { [ "$${AWS_PROFILE}" == "$(PROFILE_PRODUCTION)" ] && \
	echo s3://$(BUCKET_KEYS_PRODUCTION) || \
	echo s3://$(BUCKET_KEYS_DEVELOPMENT); } || exit 1; )

ECR_URL := \
	$(shell \
	[ -n "$${AWS_PROFILE}" ] && { [ "$${AWS_PROFILE}" == "$(PROFILE_PRODUCTION)" ] && \
	echo $(ID_PRODUCTION).dkr.ecr.us-east-1.amazonaws.com || \
	echo $(ID_DEVELOPMENT).dkr.ecr.us-east-1.amazonaws.com; } || exit 1; )

STACK = \
	$(shell echo "$*")

YAML_FILE = \
	$(shell \
	echo "$*.yaml")

SECRET_DB = \
	$(shell echo $${AWS_SECRET_DB}|$(BASE64))

PAGE_PARAM := --max-items=1000
#PAGE_PARAM := --no-paginate
#--------------------------------------------------------

help:
	@ echo ""
	@ echo "deploy CloudFormation Stack (${AWS_PROFILE})"
	@ echo ""
	@ echo "- edit environment variables    : vi setenv"
	@ echo "- set environment variables     : . setenv"
	@ echo "- verify YAML file syntax       : make lint stackname.yaml"
	@ echo "- verify all YAML file syntax   : make lint"
	@ echo "- upload template               : make stackname-template.yaml"
	@ echo "- upload env file               : make stackname.env"
	@ echo "- deploy stack                  : make stackname.yaml"
	@ echo "- remove template               : make clean stackname-template.yaml"
	@ echo "- remove env file               : make clean stackname.env"
	@ echo "- undeploy stack                : make clean stackname.yaml"
	@ echo "- list deployed stacks          : make status"
	@ echo "- list deployed resources       : make resources"
	@ echo "- list deployed stacks outputs  : make outputs"
	@ echo ""

#--------------------------------------------------------

.PHONY: .phony_explicit
.phony_explicit:

#--------------------------------------------------------

.sequence-%.pass: .phony_explicit
	@ { [ -f "$(SEQUENCE)" ] && { echo $*|$(GREP) -v -w -f $(SEQUENCE)>>$(SEQUENCE) || true; } || echo $*>>$(SEQUENCE); }

#--------------------------------------------------------

.check-env-vars:
	@ { [ -n "${AWS_PROFILE}" ] || { echo "### Environment variable AWS_PROFILE not set" && exit 1; }; }

.check-yaml-%.yaml:
	@ $(MAKE) .check-yaml-$*

.check-yaml-%:
	@ { [ -f $*.yaml ] || \
	{ echo "### YAML File $*.yaml does not exist" && exit 1; }; }

.check-file-%:
	@ { [ -f $* ] || \
	{ echo "### File $* does not exist" && exit 1; }; }

#--------------------------------------------------------

-include ./Makefile-include.mk

#--------------------------------------------------------

lint: .phony_explicit
ifeq ($(firstword $(MAKECMDGOALS)),lint)
ifneq ($(lastword $(MAKECMDGOALS)),lint)
	@ $(MAKE) .lint-$(lastword $(MAKECMDGOALS)).pass
else
	@ $(MAKE) $(LINTALL)
endif
endif

.lint-%.yaml.pass: .phony_explicit
	@ $(MAKE) .lint-$*

.lint-%: .check-yaml-%
	@ echo "--- linting $(YAML_FILE)"
	@ cfn-lint $(YAML_FILE) -i W6001 -i W2001 -i W2501 -i W8003

#--------------------------------------------------------

status: .check-env-vars
	@ rm -f STATUS STATUS_OUT STATUS_FINAL
	@ echo "--- status (${AWS_PROFILE})"
	@ { [ -f "$(SEQUENCE)" ] && cat $(SEQUENCE)>STATUS || touch STATUS; \
	[ -f "$(SEQUENCE)" ] && { ls -1 *.yaml|$(GREP) -v -f $(SEQUENCE)>>STATUS || true; } || { ls -1 *.yaml>>STATUS; }; \
	aws cloudformation describe-stacks $(PAGE_PARAM)| \
	jq -r '.Stacks[]|.StackName, .StackStatus, .LastUpdatedTime'| \
	tr \\n \\0|xargs -n 3 -0 printf "%-30s %-18s %-s\n">STATUS_OUT; \
	[ -n "$$(aws s3 ls|$(GREP) $(S3_BUCKET_CFN))" ] && { aws s3 ls $(S3_BUCKET_CFN_URL)|tr -s ' ' '\t'|cut -f1,2,4|$(SED) 's/.yaml//'|awk '{print $$3" "$$1"T"$$2}'|xargs -n2 printf "%-48s  %-s\n" >> STATUS_OUT; } || true; \
	[ -s STATUS ] && [ -s STATUS_OUT ] && { cat STATUS|$(SED) 's/.yaml//g'|xargs -n1 -I{} bash -c '$(GREP) "$$0 " STATUS_OUT >> STATUS_FINAL || echo "* $$0" >> STATUS_FINAL' {}; true; } || cat STATUS > STATUS_FINAL; \
	[ -n "$${AWS_PROFILE}" ] && [ "$${AWS_PROFILE}" == "$(PROFILE_PRODUCTION)" ] && { cat STATUS_FINAL|$(GREP) -E -v '.*-dev$$|.*-dev\s|.*-stg$$|.*-stg\s|.*-qa[0-9]$$|.*-qa[0-9]\s'; } || true; \
	[ -n "$${AWS_PROFILE}" ] && [ "$${AWS_PROFILE}" == "$(PROFILE_DEVELOPMENT)" ] && { cat STATUS_FINAL|$(GREP) -E -v '.*-prd$$|.*-prd\s'; } || true; }
	@ rm -f STATUS STATUS_OUT STATUS_FINAL

resources: .check-env-vars
	@ echo "--- resources (${AWS_PROFILE})"
	@ { aws cloudformation describe-stacks $(PAGE_PARAM) |\
	jq -r --arg LIST "$(ALLSEQREGEX)" '.Stacks[]|select(.StackName|test($$LIST))|.StackName' | xargs -n 1 aws cloudformation describe-stack-resources --stack-name |\
	jq -r '.StackResources[]|.LogicalResourceId, .ResourceType, .StackName' |\
	tr -s '\n' ' '|xargs -n 3 printf "%-45s %-45s %-s\n"; } | sort

resources-all: .check-env-vars
	@ echo "--- resources (${AWS_PROFILE})"
	@ { aws cloudformation describe-stacks $(PAGE_PARAM) |\
	jq -r '.Stacks[]|.StackName' | xargs -n 1 aws cloudformation describe-stack-resources --stack-name |\
	jq -r '.StackResources[]|.LogicalResourceId, .ResourceType, .StackName' |\
	tr -s '\n' ' '|xargs -n 3 printf "%-45s %-45s %-s\n"; } | sort

outputs: .check-env-vars
	@ echo "--- outputs (${AWS_PROFILE})"
	@ { aws cloudformation describe-stacks $(PAGE_PARAM) |\
	jq -r --arg LIST "$(ALLSEQREGEX)" '.Stacks[]|.StackName as $$SN|select(.StackName|test($$LIST))|.Outputs|arrays|.[]|select(.ExportName|strings)|.ExportName+" "+.OutputValue' |\
	$(SED) "s/ /!/"|tr '!' '\0'|tr '\n' '\0'|xargs -0 -n2 printf "%-60s %-s\n"; } | sort

outputs-all: .check-env-vars
	@ echo "--- outputs (${AWS_PROFILE})"
	@ { aws cloudformation describe-stacks $(PAGE_PARAM) |\
	jq -r '.Stacks[]|.Outputs|arrays|.[]|select(.ExportName|strings)|.ExportName+" "+.OutputValue' |\
	$(SED) "s/ /!/"|tr '!' '\0'|tr '\n' '\0'|xargs -0 -n2 printf "%-60s %-s\n"; } | sort

#--------------------------------------------------------

.log-%: .check-env-vars
	@ echo "--- log $(YAML_FILE) (${AWS_PROFILE})"
	@ { aws cloudformation describe-stack-events $(PAGE_PARAM) --stack-name $* 2>/dev/null| \
	$(GREP) -A5 'CREATE_FAILED'|$(GREP) --color=never 'Timestamp'|tr -d \ |cut -c13-46|xargs -n1 -I{} \
	bash -c 'aws cloudformation describe-stack-events $(PAGE_PARAM) --stack-name $* 2>/dev/null|jq -r ".StackEvents[]|select(.Timestamp == \"$0\")|{ResourceStatusReason},.ResourceProperties"' {} | \
	$(SED) 's/\\//g'|jq; }

#--------------------------------------------------------

.upload-template-%: .check-env-vars
	@ echo "--- uploading $* (${AWS_PROFILE})"
	@ aws s3 cp $* $(S3_BUCKET_CFN_URL)

.remove-template-%: .check-env-vars
	@ echo "--- removing $*-template (${AWS_PROFILE})"
	@ { aws s3 rm $(S3_BUCKET_CFN_URL)/$*-template.yaml || true; }

#--------------------------------------------------------

.upload-env-%: .check-env-vars
	@ echo "--- uploading $* (${AWS_PROFILE})"
	@ aws s3 cp $* $(S3_BUCKET_CFN_URL)

.remove-env-%: .check-env-vars
	@ echo "--- removing $*.env (${AWS_PROFILE})"
	@ { aws s3 rm $(S3_BUCKET_CFN_URL)/$*.env || true; }

#--------------------------------------------------------

define PARAMETER_OVERRIDES
[ \
{"ParameterKey": "AwsProfile", "ParameterValue": "${AWS_PROFILE}"}, \
{"ParameterKey": "Org", "ParameterValue": "$(ORG)"}, \
{"ParameterKey": "Db", "ParameterValue": "$(SECRET_DB)"}, \
{"ParameterKey": "TimeStamp", "ParameterValue": "${TIME_FIX}"}, \
{"ParameterKey": "MasterUserPassword", "ParameterValue": "${AWS_MASTER_USER_PASSWORD}"} \
]
endef

.deploy-%: .check-env-vars .lint-%
	@ echo "--- deploying CloudFormation Stack $(STACK) (${AWS_PROFILE})"
	@ aws cloudformation deploy \
	--stack-name $(STACK) \
	--template-file $(YAML_FILE) \
	--capabilities CAPABILITY_NAMED_IAM CAPABILITY_AUTO_EXPAND \
	--parameter-overrides '$(PARAMETER_OVERRIDES)'
	@ $(MAKE) .post-deploy-$*

.post-deploy-%:
	@ true

%-template.yaml: .phony_explicit
ifneq ($(firstword $(MAKECMDGOALS)),lint)
ifneq ($(firstword $(MAKECMDGOALS)),clean)
	@ $(MAKE) .lint-$@.pass
	@ $(MAKE) .upload-template-$@
	@ $(MAKE) .sequence-$@.pass
endif
endif

%.env: .phony_explicit
ifneq ($(firstword $(MAKECMDGOALS)),lint)
ifneq ($(firstword $(MAKECMDGOALS)),clean)
	@ $(MAKE) .upload-env-$@
	@ $(MAKE) .sequence-$@.pass
endif
endif

%.yaml: export TIME_FIX=$(TIME_STAMP)
%.yaml: .phony_explicit
ifneq ($(firstword $(MAKECMDGOALS)),lint)
ifneq ($(firstword $(MAKECMDGOALS)),clean)
	@ $(MAKE) .deploy-$*
	@ $(MAKE) .sequence-$*.yaml.pass
endif
endif

all: .phony_explicit
	@ $(MAKE) $(ALLSEQ)

#--------------------------------------------------------

.pre-undeploy-%:
	@ true

.undeploy-%-template: .phony_explicit
	@ $(MAKE) .remove-template-$*

.undeploy-%-env: .phony_explicit
	@ $(MAKE) .remove-env-$*

.undeploy-%: .check-env-vars
	@ $(MAKE) .pre-undeploy-$(STACK)
	@ echo "--- undeploying CloudFormation Stack $(STACK) (${AWS_PROFILE})"
	@ aws cloudformation delete-stack --stack-name $(STACK)
	@ { while [ -n "$$(aws cloudformation describe-stacks $(PAGE_PARAM)|jq -r '.Stacks[]|select(.StackName=="$(STACK)")|.StackName')" ]; do \
	aws cloudformation describe-stacks $(PAGE_PARAM)|jq -r '.Stacks[]|select(.StackName=="$(STACK)")|{StackName, StackStatus}'; done; }

clean: .phony_explicit
ifeq ($(firstword $(MAKECMDGOALS)),clean)
ifneq ($(lastword $(MAKECMDGOALS)),clean)
	@ $(MAKE) .clean-$(lastword $(MAKECMDGOALS)).pass
endif
endif

.clean-%.yaml.pass: .phony_explicit
	@ $(MAKE) .clean-$*

.clean-%.env.pass: .phony_explicit
	@ $(MAKE) .undeploy-$*-env

.clean-%: .phony_explicit
	@ $(MAKE) .undeploy-$(STACK)

clean-all: .phony_explicit
	@ echo "--- clean all (${AWS_PROFILE}) - $(ALLREV)"
	@ $(MAKE) $(ALLREV)

#--------------------------------------------------------

.SILENT:
