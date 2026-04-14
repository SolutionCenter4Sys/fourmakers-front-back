# ecs update-service --cluster ecs --service srv-curriculo-batch-consumer-dev --enable-execute-command --force-new-deployment
export TASK=$(aws ecs list-tasks --cluster ecs --service-name srv-curriculo-batch-consumer-dev|jq -r '.taskArns[]'|head -n 1); aws ecs execute-command --cluster ecs --task  ${TASK} --interactive --command '/bin/sh'
