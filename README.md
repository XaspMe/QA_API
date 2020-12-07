<h4>C# Core 3.1 Full REST API training project.
<br><br>
The project allows:<br>
Using API:<br><br>

*  Add / display a question and answer on a specific topic.
*  Add / display topics for questions.
*  The application can be used for theoretical preparation before tests or interviews. For exams or certification.<br>

There is no client because the project is used for practice in studying the technology used and is a prototype
<br><br>	
The project includes:

* Automapper
* MVC;
* REST;
* the Repository Pattern;
* Dependency Injection; 
* Entity Framework;
* Data Transfer Objects (DTOs);
* AutoMapper to provide API endpoints

<h4>Examples for POST:<br><br>

﻿Create Element

	/api/Elements/
	{
		"Question": "OOP20",
		"Answer": "OOP17",
		"CategoryName": "OOP"
	}

Create Category

	/api/Categoties/
	{
		"Name": "Python"
	}
