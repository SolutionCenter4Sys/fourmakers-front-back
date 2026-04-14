import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { RegrasTab } from "@presentation/components/reembolso-parametros/RegrasTab";
import { RegrasPersonalizadasTab } from "@presentation/components/reembolso-parametros/RegrasPersonalizadasTab";
import { LogTab } from "@presentation/components/reembolso-parametros/LogTab";
import { Settings, FileText, Receipt } from "@/components/ui/system-icons";

const ReembolsoParametros = () => {
  return (
    <div className="min-h-screen bg-primaryBackground">
      <div className="container py-8">
        <h1 className="page-title mb-8">Reembolso parâmetros</h1>
        
        <Tabs defaultValue="regras" className="w-full">
          <div className="mb-6">
            <TabsList className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-xl gap-2 w-auto shadow-sm">
              <TabsTrigger 
                value="regras" 
                className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
              >
                <Settings className="h-4 w-4" />
                <span>Regras</span>
              </TabsTrigger>
              <TabsTrigger 
                value="personalizadas" 
                className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
              >
                <FileText className="h-4 w-4" />
                <span>Regras personalizadas</span>
              </TabsTrigger>
              <TabsTrigger 
                value="log" 
                className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
              >
                <Receipt className="h-4 w-4" />
                <span>Log</span>
              </TabsTrigger>
            </TabsList>
          </div>
          
          <TabsContent value="regras">
            <RegrasTab />
          </TabsContent>
          
          <TabsContent value="personalizadas">
            <RegrasPersonalizadasTab />
          </TabsContent>
          
          <TabsContent value="log">
            <LogTab />
          </TabsContent>
        </Tabs>
      </div>
    </div>
  );
};

export default ReembolsoParametros;
