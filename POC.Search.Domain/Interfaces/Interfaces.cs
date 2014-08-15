public interface IApplicationService
{
    void Execute(ICommand cmd);
}

public interface IEvent { }

public interface ICommand { }

public interface IIdentity { }
