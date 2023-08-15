@echo off

docker build -t tradex-event-publisher:1.0.0 -f ../src/EventPublisher/Dockerfile ../src && helm upgrade tradex-event-publisher ../charts/tradex-event-publisher --install --recreate-pods

