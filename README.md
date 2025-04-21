## AdministrationDB

![AdministrationDB](img/01.png)
![AdministrationDB](img/02.png)
![AdministrationDB](img/03.png)
![AdministrationDB](img/04.png)
![AdministrationDB](img/05.png)
![AdministrationDB](img/06.png)
![AdministrationDB](img/07.png)
![AdministrationDB](img/08.png)
![AdministrationDB](img/09.png)
![AdministrationDB](img/10.png)
![AdministrationDB](img/10.png)


## Program
```cs
string connectionString = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddDbContext<BDContext>(options =>
    options.UseSqlite(connectionString));
``` 

## appsetting.json
```cs
{
  "ConnectionStrings": {
    "Connection": "Data Source=AdministrationDB.db"
}
``` 