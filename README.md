# C# Core 3.1 Full REST API training project.

The project allows:

  - Add / display a question and answer on a specific topic.
  - Add / display topics for questions.
  - The application can be used for theoretical preparation before tests or interviews. For exams or certification.

There is no client, because the project is a prototype and is used for practice in the study of applied technologies.

### Tech

The project includes:

* [Git] - For versioning
* [Automapper] - To comply with the client's contract and to provide API endpoints
* MVC;
* REST;
* The Repository Pattern;
* Dependency Injection; 
* [EF] - Entity Framework for work with MsSQL;
* Data Transfer Objects (DTOs);

### Examples for POST
Create Element body

	/api/Elements/
	{
		"Question": "OOP20",
		"Answer": "OOP17",
		"CategoryName": "OOP"
	}

Create Category body

	/api/Categoties/
	{
		"Name": "Python"
	}

[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job. There is no need to format nicely because it shouldn't be seen. Thanks SO - http://stackoverflow.com/questions/4823468/store-comments-in-markdown-syntax)


   [Git]: <https://git-scm.com/>
   [Automapper]: <https://automapper.org/>
   [EF]: https://docs.microsoft.com/en-us/ef/>
   
