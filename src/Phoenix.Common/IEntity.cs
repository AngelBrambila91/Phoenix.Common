namespace Phoenix.Common;
/*
If we take a look at our GodsRepository class we will notice that there’s a lot of code that we will need
to reuse in future microservices. Same for the MongoDb and Service Settings classes.
But before we can move anything to a new shared library we will need to do some good refactoring to
keep the generic pieces separated from what is really needed in the Catalog microservice.
*/
public interface IEntity
{
    Guid Id { get; set; }
}