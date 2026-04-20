import { useState, useEffect } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { Users, Eye, Download, User, Plus } from "@/components/ui/system-icons";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { AlocacoesTab } from "@presentation/components/mapa-alocacao/AlocacoesTab";
import { VisaoGerencialTab } from "@presentation/components/mapa-alocacao/VisaoGerencialTab";
import { TBDTab } from "@presentation/components/mapa-alocacao/TBDTab";
import { ExportarRelatorioModal } from "@presentation/components/mapa-alocacao/ExportarRelatorioModal";
import { ChatIan } from "@presentation/components/mapa-alocacao/ChatIan";
import { useAppSelector } from "@/app/store/hooks";
import type { ListarAlocacoesPayload } from "@domain/entities/MapaAlocacao";
import { Button } from "@/components/ui/button";

const STORAGE_KEY = 'mapa-alocacao-search-state';

const tabUrlToValue: Record<string, string> = {
  'tbd': 'tbd',
  'visaogerencial': 'visao-gerencial',
};

const tabValueToUrl: Record<string, string> = {
  'tbd': 'tbd',
  'visao-gerencial': 'visaogerencial',
};

const MapaAlocacao = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();
  const { user } = useAppSelector((state) => state.auth);
  const [modalExportarOpen, setModalExportarOpen] = useState(false);
  
  // Verificar se o usuário tem a funcionalidade RELATORIO_ALOCACOES ativa
  const temRelatorioAlocacoes = user?.funcionalidadeSistema?.some(
    (func) => func.descricao === "RELATORIO_ALOCACOES" && func.ativo === true
  ) ?? false;

  // Verificar se o usuário tem a funcionalidade CRIAR_RECURSO_TBD ativa
  const temCriarRecursoTbd = user?.funcionalidadeSistema?.some(
    (func) => func.descricao === "CRIAR_RECURSO_TBD" && func.ativo === true
  ) ?? false;
  
  const [activeTab, setActiveTab] = useState<string>(() => {
    const tabParam = searchParams.get('tab');
    if (tabParam && tabUrlToValue[tabParam]) {
      return tabUrlToValue[tabParam];
    }
    // Se não encontrar mapeamento, verifica se é visao-gerencial diretamente
    if (tabParam === 'visaogerencial') {
      return 'visao-gerencial';
    }
    return 'alocacoes';
  });

  useEffect(() => {
    const tabParam = searchParams.get('tab');
    let tabFromUrl = 'alocacoes';
    
    if (tabParam) {
      if (tabUrlToValue[tabParam]) {
        tabFromUrl = tabUrlToValue[tabParam];
      } else if (tabParam === 'visaogerencial') {
        tabFromUrl = 'visao-gerencial';
      }
    }
    
    // Se tentar acessar TBD sem permissão, redirecionar para alocacoes
    if (tabFromUrl === 'tbd' && !temCriarRecursoTbd) {
      setSearchParams({});
      setActiveTab('alocacoes');
      return;
    }
    
    if (tabFromUrl !== activeTab) {
      setActiveTab(tabFromUrl);
    }
  }, [searchParams, activeTab, temCriarRecursoTbd, setSearchParams]);

  const handleTabChange = (value: string) => {
    setActiveTab(value);
    const urlTab = tabValueToUrl[value];
    const newParams = new URLSearchParams(searchParams);
    
    if (urlTab) {
      newParams.set('tab', urlTab);
      // Se não for visao-gerencial, remover o parâmetro view
      if (value !== 'visao-gerencial') {
        newParams.delete('view');
      }
    } else {
      newParams.delete('tab');
      newParams.delete('view');
    }
    
    setSearchParams(newParams);
  };

  // Obter filtros atuais do localStorage
  const getFiltrosAtuais = (): ListarAlocacoesPayload | undefined => {
    try {
      const saved = localStorage.getItem(STORAGE_KEY);
      if (saved) {
        const parsed = JSON.parse(saved);
        if (parsed.filtros) {
          return parsed.filtros;
        }
      }
    } catch (e) {
      console.error("Erro ao obter filtros do localStorage:", e);
    }
    return undefined;
  };
  return (
    <div className="container mx-auto p-6 space-y-6">
      <h1 className="page-title">Mapa de Alocação</h1>
      
      <Tabs value={activeTab} onValueChange={handleTabChange} className="w-full">
        <div className="mb-6 flex items-center justify-between">
          <TabsList className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-xl gap-2 w-auto shadow-sm">
            <TabsTrigger 
              value="alocacoes" 
              className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              <Users className="h-4 w-4" />
              <span>Alocações</span>
            </TabsTrigger>
            <TabsTrigger 
              value="visao-gerencial" 
              className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
            >
              <Eye className="h-4 w-4" />
              <span>Visão gerencial</span>
            </TabsTrigger>
            {temCriarRecursoTbd && (
              <TabsTrigger 
                value="tbd" 
                className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
              >
                <User className="h-4 w-4" />
                <span>TBD</span>
              </TabsTrigger>
            )}
          </TabsList>
          
          {activeTab === 'alocacoes' && (
            <div className="flex items-center gap-2">
              {temRelatorioAlocacoes && (
                <Button 
                  variant="outline" 
                  className="gap-2"
                  onClick={() => setModalExportarOpen(true)}
                >
                  <Download className="h-4 w-4" />
                  Exportar
                </Button>
              )}
              <Button 
                className="gap-2"
                onClick={() => navigate("/mapa-alocacao/nova")}
              >
                <Plus className="h-4 w-4" />
                Criar alocação
              </Button>
            </div>
          )}
        </div>
        
        <TabsContent value="alocacoes">
          <AlocacoesTab />
        </TabsContent>
        
        <TabsContent value="visao-gerencial">
          <VisaoGerencialTab />
        </TabsContent>
        
        {temCriarRecursoTbd && (
          <TabsContent value="tbd">
            <TBDTab />
          </TabsContent>
        )}
      </Tabs>

      <ExportarRelatorioModal
        open={modalExportarOpen}
        onOpenChange={setModalExportarOpen}
        filtrosAtuais={getFiltrosAtuais()}
      />
      
      {/* Chat Ian */}
      <ChatIan />
    </div>
  );
};

export default MapaAlocacao;
