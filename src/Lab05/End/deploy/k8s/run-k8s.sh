#!/bin/bash

kubectl apply -f gateway-deployment.yaml
kubectl apply -f gateway-service.yaml
kubectl apply -f breadservice-deployment.yaml
kubectl apply -f breadservice-service.yaml
kubectl apply -f cheeseservice-deployment.yaml
kubectl apply -f cheeseservice-service.yaml
kubectl apply -f lettuceservice-deployment.yaml
kubectl apply -f lettuceservice-service.yaml
kubectl apply -f meatservice-deployment.yaml
kubectl apply -f meatservice-service.yaml
kubectl apply -f sandwichmaker-deployment.yaml
kubectl apply -f sandwichmaker-service.yaml