import { useState } from 'react';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { PageBreadcrumb, PageHeader } from '@presentation/components/common';
import { Users, FileText, Settings } from '@/components/ui/system-icons';
import { GruposTab } from '@presentation/components/permissionamento/GruposTab';
import { AuditoriaTab } from '@presentation/components/permissionamento/AuditoriaTab';
import { FuncionalidadesSistemaTab } from '@presentation/components/permissionamento/FuncionalidadesSistemaTab';

export default function Permissionamento() {
  const [activeTab, setActiveTab] = useState('grupos');

  return (
    <div className="container mx-auto p-4 space-y-6">
      <PageBreadcrumb 
        items={[
          { label: 'Gestão', href: '#' },
          { label: 'Gestão de Acessos' }
        ]}
      />

      <PageHeader
        title="Gestão de Acessos"
        description="Controle de perfis, grupos e permissões"
      />

      <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
        <TabsList className="inline-flex h-auto bg-muted/30 backdrop-blur-sm border border-border/50 p-1.5 rounded-xl gap-2 w-auto shadow-sm">
          <TabsTrigger
            value="grupos"
            className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
          >
            <Users className="h-4 w-4" />
            <span>Grupos de Acesso</span>
          </TabsTrigger>

          <TabsTrigger
            value="funcionalidades"
            className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
          >
            <Settings className="h-4 w-4" />
            <span>Funcionalidades do Sistema</span>
          </TabsTrigger>

          <TabsTrigger
            value="auditoria"
            className="relative flex items-center gap-2.5 text-sm font-medium py-2.5 px-5 rounded-lg transition-all duration-200 data-[state=inactive]:text-muted-foreground data-[state=inactive]:hover:text-foreground data-[state=inactive]:hover:bg-muted/50 data-[state=active]:bg-primary data-[state=active]:text-primary-foreground data-[state=active]:shadow-md data-[state=active]:scale-[1.02]"
          >
            <FileText className="h-4 w-4" />
            <span>Auditoria</span>
          </TabsTrigger>
        </TabsList>

        <TabsContent value="grupos" className="mt-6">
          <GruposTab />
        </TabsContent>

        <TabsContent value="funcionalidades" className="mt-6">
          <FuncionalidadesSistemaTab />
        </TabsContent>

        <TabsContent value="auditoria" className="mt-6">
          <AuditoriaTab />
        </TabsContent>
      </Tabs>
    </div>
  );
}
