# Makefile CloueFormation IaC
# Mastria

#--------------------------------------------------------

info:
	@ echo ""
	@ echo "deploy CloudFormation Stack (${AWS_PROFILE})"
	@ echo ""
	@ echo "*** não executar este Makefile manualmente ***"
	@ echo ""

#--------------------------------------------------------

.post-deploy-api-design-task-%: export LATEST_IMAGE=\
	$(shell \
	[ "$*" != "" ] && { aws ecr list-images --repository-name blueprints/api-design/$* |\
	jq -r '.imageIds[]|select(.imageTag == "latest")|.imageDigest'; } )

.post-deploy-api-design-task-%: .check-env-vars
	@ { [ ! -n "$(LATEST_IMAGE)" ] && { \
	echo "--- post-deploy - push Registry Image $(LATEST_IMAGE)"; \
	aws ecr get-login-password|docker login --username AWS --password-stdin $(ECR_URL); \
	docker pull $(ECR_URL)/devops/landing:latest; \
	docker tag $(ECR_URL)/devops/landing:latest $(ECR_URL)/blueprints/api-design/$*:latest; \
	docker push $(ECR_URL)/blueprints/api-design/$*:latest; \
	docker rmi $(ECR_URL)/devops/landing:latest; \
	docker rmi $(ECR_URL)/blueprints/api-design/$*:latest; } || true; }

.post-deploy-api-design-task-%: .phony_explicit
	@ true

#--------------------------------------------------------

.pre-undeploy-api-design-task-%: export NESTED_STACK=\
	$(shell \
	[ -n "$(STACK)" ] && { aws cloudformation describe-stack-resources --stack-name api-design-task-$(STACK) |\
	jq -r '.StackResources[]|select(.ResourceType=="AWS::CloudFormation::Stack")|.PhysicalResourceId'; } || echo ""; )

.pre-undeploy-api-design-task-%: export REPOSITORY_NAME=\
	$(shell \
	[ -n "$(NESTED_STACK)" ] && [ "$(NESTED_STACK)" != "null" ] && { aws cloudformation describe-stack-resources --stack-name $(NESTED_STACK) |\
	jq -r '.StackResources[]|select(.ResourceType=="AWS::ECR::Repository")|.PhysicalResourceId'; } || echo ""; )

.pre-undeploy-api-design-task-%: .check-env-vars
	@ echo "--- pre-undeploy - cleanup Registry Images $(REPOSITORY_NAME)"
	@ { [ -n "$(REPOSITORY_NAME)" ] && { aws ecr list-images --repository-name $(REPOSITORY_NAME) |\
	jq -r ".imageIds[]|.imageTag"|xargs -n1 -I{} \
	aws ecr batch-delete-image --repository-name $(REPOSITORY_NAME) --image-ids imageTag={}; } || true; }
	@ { [ -n "$(REPOSITORY_NAME)" ] && { aws ecr list-images --repository-name $(REPOSITORY_NAME) |\
	jq -r ".imageIds[]|.imageDigest"|xargs -n1 -I{} \
	aws ecr batch-delete-image --repository-name $(REPOSITORY_NAME) --image-ids imageDigest={}; } || true; }
