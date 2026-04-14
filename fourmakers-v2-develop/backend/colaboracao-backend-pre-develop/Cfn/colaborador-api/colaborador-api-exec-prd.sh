# ecs update-service --cluster ecs --service srv-colaborador-api-prd --enable-execute-command --force-new-deployment
export TASK=$(aws ecs list-tasks --cluster ecs --service-name srv-colaborador-api-prd|jq -r '.taskArns[]'|head -n 1); aws ecs execute-command --cluster ecs --task  ${TASK} --interactive --command '/bin/sh'
