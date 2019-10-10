using Aspenlaub.Net.GitHub.CSharp.Paleface.Components;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.PressEnter {
    public static class PressEnterContainerBuilder {
        public static ContainerBuilder UsePressEnterAndPaleface(this ContainerBuilder builder) {
            builder.UsePaleface();
            builder.RegisterType<PressEnterAgent>().As<IPressEnterAgent>();
            return builder;
        }
    }
}
