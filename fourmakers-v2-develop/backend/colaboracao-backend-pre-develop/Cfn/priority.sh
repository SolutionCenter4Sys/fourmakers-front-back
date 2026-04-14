. setenv
export LOAD_BALANCER=$(aws elbv2 describe-load-balancers|jq -r '.LoadBalancers[]|select(.Type == "application")|.LoadBalancerArn')
echo "--- load balancer: ${LOAD_BALANCER}"
export LISTENER_ARN=$(aws elbv2 describe-listeners --load-balancer ${LOAD_BALANCER}|jq -r '.Listeners[]|select(.Protocol=="HTTPS")|.ListenerArn')
echo "--- listener: ${LISTENER_ARN}"
echo "--- aws:"
aws elbv2 describe-rules --listener-arn ${LISTENER_ARN}|jq -r '.Rules[]|select(.IsDefault==false)|{Priority, Domain: .Conditions[0].Values[0]}|.Priority+" "+.Domain'|sort -n
echo "--- local:"
ggrep -A2 '^ *Priority:$' *.yaml|ggrep 'Default'|sed 's/.yaml.*Default/ /g'
echo "--- next:"
aws elbv2 describe-rules --listener-arn ${LISTENER_ARN}|jq -r '.Rules[]|select(.IsDefault==false)|.Priority'| jq -r --slurp '.|max'|xargs echo '1 +'|bc
