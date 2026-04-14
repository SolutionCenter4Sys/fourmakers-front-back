using Rotina.Aws.Core;
using Rotina.Aws.Core.Interfaces;
using Rotina.Aws.Domain.Impl;
using Rotina.CuradoriaSkill.Application;

IAppBuilder builder = new AppBuilder();
IInitializer initializer = new Initializer();

return await builder.CreateJobBuilder<CuradoriaSkillJobService>(args, initializer);