# TopUp Feature

## How to run the test:


## Design (TopUpService)
 - A simple 3 layer archtecture inspired by the 4 tenets of Onion Archtecture (is not "by the book", it's just a 'inspiration')
 - ![image](https://github.com/cassio-morais/technical-assessment-topup-feature/assets/63246083/4f21c3e7-88e5-438d-958e-4d610a7f6805)
 - Ref: https://jeffreypalermo.com/2013/08/onion-architecture-part-4-after-four-years/
 - ps: I like this approach. It's simple but it's interfaces oriented and has great testability. 

## Disclaimers:
 - There are comments in code (to explain some of my point of views in real world solutions)
 - There so many challenges in a real world solution like this: 
	- Distributed transactions
	- Distributed locks
	- Commit and Rollback actions
	- Events
	- Audit... etc
 - So... none of these challenges were addressed here. But I'm aware of them.

