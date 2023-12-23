using System.Globalization;
using System.Reflection;
using System.Xml.Serialization;
using LamLibAllOver;

namespace Ground.Share.Lib.Package;

[AttributeUsage(AttributeTargets.Method)]
public class AttributePackageBundleBuild: Attribute {
    private readonly Type _classType;
    private readonly string _methodName;
    
    public async Task<SResult<IPackageBundle>> GetBuilderAsync() { ;
        try {
            var methodInfo = _classType
                             .GetMethods()
                             .FirstOrDefault(x => x.IsStatic
                                                  && x.ReturnType == typeof(Task<IPackageBundle>)
                                                  && x.Name == _methodName);

            if (methodInfo is null) {
                return SResult<IPackageBundle>.Err(TraceMsg.WithMessage("MethodInfo Not Found"));
            }

            var returnValue = methodInfo.Invoke(
                null,
                BindingFlags.Static,
                null,
                null,
                CultureInfo.DefaultThreadCurrentCulture
            );

            if (returnValue is null) {
                return SResult<IPackageBundle>.Err(TraceMsg.WithMessage("Function Return null"));
            }
            
            if (returnValue.GetType() == typeof(Task<IPackageBundle>)) {
                return SResult<IPackageBundle>.Err(
                    TraceMsg.WithMessage("Function Return False Type Must Be 'Task<IPackageBundle>'")
                );
            }

            return SResult<IPackageBundle>.Ok(await (Task<IPackageBundle>)returnValue);
        }
        catch (Exception e) {
            return SResult<IPackageBundle>.Err(e);
        }
    }

    public AttributePackageBundleBuild(Type classType, string methodName) {
        _classType = classType;
        _methodName = methodName;
    }
}