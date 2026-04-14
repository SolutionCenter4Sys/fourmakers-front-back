# ecs update-service --cluster ecs --service srv-bi-api-dev --enable-execute-command --force-new-deployment
export TASK=$(aws ecs list-tasks --cluster ecs --service-name srv-bi-api-dev|jq -r '.taskArns[]'|head -n 1); aws ecs execute-command --cluster ecs --task  ${TASK} --interactive --command '/bin/sh'
