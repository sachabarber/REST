
http://www.asp.net/web-api/overview/creating-web-apis/creating-a-web-api-that-supports-crud-operations


+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
URI				Method		Description							Output					Input					StatusCode
+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
/people			GET			All people in the system			person collection		n/a						200/404
/people/{id}	GET			Get specific person					person					n/a						200/404
/people			POST		Creates a new entity in the			person					person (without the		201/404
							system. Expects a representation							id specified)
							of the person in the body
/people/{id}	PUT			Modifies a person resource			n/a						person					200/404
/people/{id}	DELETE		Deletes a person from the system	n/a						person					200/404



