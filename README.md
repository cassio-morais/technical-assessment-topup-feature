# TopUp Feature

## How to run:
 - Install Docker and Docker Compose
 - Browse to the project root directory  
 - Run `docker-compose up -d`
 - Swagger (TopUpService): http://localhost:8080/swagger/index.html
 - Swagger (AccountService): http://localhost:8082/swagger/index.html (just an bank account simulation)

## Problem Solution 
 - Big Picture
 - ![image](https://github.com/cassio-morais/technical-assessment-topup-feature/assets/63246083/f7a43cea-91fb-4e32-9a58-d0358b8d22fc)

## Design (TopUpService)
 - A simple 3 layer archtecture inspired by the 4 tenets of Onion Archtecture (is not "by the book", it's just a 'inspiration')
 - ![image](https://github.com/cassio-morais/technical-assessment-topup-feature/assets/63246083/cec462c0-b3be-45ee-9ddb-3a147de31484)
 - Ref: https://jeffreypalermo.com/2013/08/onion-architecture-part-4-after-four-years/
 - ps: I like this approach. It's simple but it's interface oriented and has great testability. 

## Disclaimers:
 - I put more effort in top-up service. Account and user service are just simulations
 - In real world, solutions like this need more unit tests and integration tests.
 - There are comments in code (to explain some of my point of view about real world solutions)
 - There are so many challenges in a real world solution like these: 
	- Distributed transactions (Saga Pattern or something)
	- Distributed locks
	- Commit and Rollback actions
 - Retry policies
	- Events
	- Audit
 - Observability
 - So... none of these challenges were addressed here. But I'm aware of them.

