namespace Titan.Core;

/// <summary>
/// the IApi interface is used to mark resources that will be acquired with the .GetApi<T>() function
/// </summary>
public interface IApi { }

/// <summary>
/// the IResource interface is used to mark resources that will be acquired with the GetMutableResource or GetReadOnlyResource
/// </summary>
public interface IResource { }

/// <summary>
/// the IComponent interface is used to mark resources that will be acquired with the GetMutableStorage or GetReadOnlyStorage
/// </summary>
public interface IComponent { }

/// <summary>
/// the IEvent interface is used to mark resources that will be acquired with the GetEventsReader or GetEventsWriter
/// </summary>
public interface IEvent { }
