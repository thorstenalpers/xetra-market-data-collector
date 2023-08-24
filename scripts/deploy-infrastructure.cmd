@echo off


helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
helm repo add superset https://apache.github.io/superset
helm repo add timescale https://charts.timescale.com
helm repo add truecharts https://charts.truecharts.org/
helm repo update

helm upgrade --install nginx --version 4.4.0 ingress-nginx/ingress-nginx -f "..\charts\nginx\values.yaml" --wait
helm upgrade --install ingress-nodeports  "..\charts\ingress-nodeports\chart" --wait
helm upgrade --install selenium "C:\Sources\TradeX\charts\selenium" --wait
helm upgrade --install superset --version 0.10.5 superset/superset -f "..\charts\superset\values.yaml" --wait
helm upgrade --install rabbitmq --version 12.0.10 bitnami/rabbitmq -f "..\charts\rabbitmq\values.yaml" --wait
helm upgrade --install timescale --version 0.13.1 timescale/timescaledb-single -f "..\charts\timescale\values.yaml" --wait
helm install jupyter truecharts/jupyter --version 5.0.8 -f "..\charts\jupyter\values.yaml" --wait
