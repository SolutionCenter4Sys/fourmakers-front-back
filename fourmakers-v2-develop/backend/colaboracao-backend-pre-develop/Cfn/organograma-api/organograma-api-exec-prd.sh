# ecs update-service --cluster ecs --service srv-organograma-api-prd --enable-execute-command --force-new-deployment
export TASK=$(aws ecs list-tasks --cluster ecs --service-name srv-organograma-api-prd|jq -r '.taskArns[]'|head -n 1); aws ecs execute-command --cluster ecs --task  ${TASK} --interactive --command '/bin/sh'
