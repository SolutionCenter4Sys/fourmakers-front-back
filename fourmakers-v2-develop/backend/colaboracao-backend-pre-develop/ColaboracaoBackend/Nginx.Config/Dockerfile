FROM nginx:1.21.4
COPY ["default.conf","/etc/nginx/conf.d/default.conf"]
#EXPOSE 84:80
ENTRYPOINT ["nginx"]
CMD ["-g", "daemon off;"]
