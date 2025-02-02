
# ChangeLog 

## Release -  Esquio 1.1 released on 2019, October 18

- Created new MiniProfiler extension for profile feature evaluation.
- UI
    - New
        - add authorization policies for Esquio.UI, we have Manager, Writer and Reader roles
        - nprogress styles
        - new buttons
        - added new NotAllowed screen when user is not authorized
    - Fixes
        - regular expressions for Esquio.Semicolon parameters
        - blur input
        - remove Newtonsoft dependency
        - fix creation toggle type, now use fully qualified name without version and public key
- Esquio
    - New
        - set a new Diagnostic strategy and consolidate logging policies.
            - created new EventData public types for DiagnosticListener integrators.
        - nnable change default partitioner for Esquio.
        - new option for EsquioOptions to configure DefaultProductName to be used when is not specified.
        - new property for DesignType metadata to specify the friendly name of each toggle.
    - Fixes
        - improve code quality.
        - fix issue with DiagnosticListener registration.
- Esquio.AspNetCore
    - New
        - Added Blazor component.
        - HeaderValueToggle for Esquio.AspNetCore.
    - Fix
        - fix lifetime registration for asp.net core services

- Esquio.EntityFrameworkCore
    - Fixes
        -Remove Newtonsoft dependency

Thanks to our external contributors Meir Blachman @Meir017, Jorge Turrado @JorTurFer for your commits on this release!!!

## Patch - Esquio.AspNetCore 1.0.1 released on 2019, October 8

- Esquio.AspNetCore 1.0.1
    - Fixing allocation on AspNetEnvironmentNameProviderService
    - Fix lifetime registration for RoleName, UserName and EnvironmentName provider services

## Patch - Esquio 1.0.1 released on 2019, October 2

- Esquio 1.0.1
    - Fixing bug on FromToToggle when use one digit on month, day or hour.

## Release - Esquio 1.0 released on 2019, September 24

- Esquio 1.0
- Esquio.AspNetCore 1.0
- Esquio.ConfigurationStore 1.0
- Esquio.EntityFrameworkStore 1.0
- MVP for Esquio.UI