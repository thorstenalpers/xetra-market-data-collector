@echo off

kubectl scale deploy tradex-marketdata --replicas=0
kubectl delete pod -l app.kubernetes.io/instance=selenium
docker build -t tradex-marketdata:1.0.0 -f ../src/MarketData/MarketData.API/Dockerfile ../src && helm upgrade tradex-marketdata ../charts/tradex-marketdata --install --recreate-pods

