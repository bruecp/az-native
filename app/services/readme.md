# Services 

To make local development and debug easier use the following ports reference for the services:

| .NET Api Services         | Http Port | Https Port | Dapr Port | Dapr App ID          | Docker Port|
| -------                   | --------- | ---------- | --------- | -------------        | -----|
| Catalog Service           | 5001      | 5021       | 5011      | catalog-service      | 5051 | 
| Order Service             | 5002      | 5022       | 5012      | order-service        | 5052 |
| Payment Service           | 5004      | 5024       | 5014      | payment-service      | 5054 |
| Bank Actor Service        | 5005      | 5025       | 3500      | bank-actor           | 5055 |
| Cooking Service           | 5006      | 5026       | 5016      | cooking-service      | 5056 |
| Delivery Service          | 5007      | 5027       | 5017      | deliver-service      | 5057 |
| Graph NotificationService | 5008      | 5028       | 5018      | notification-service | 5058 |

 
| Azure Functions                 | Http Port | Docker Port|
| -------                         | --------- | ---------- | 
| Order Event Processor Function  | 7073      |            |	
| Payment Service Function        | 7074      |            | 
| Cooking Dashboard Function      | 7076      |            |
| Optimizer Function              | 7077      |            |
| Invoices Job Function           | 7078      |            |