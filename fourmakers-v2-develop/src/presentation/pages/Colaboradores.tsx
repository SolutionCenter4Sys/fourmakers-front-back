import { useState, useEffect, useCallback, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import { Search, Plus, User, Edit, Download } from "@/components/ui/system-icons";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { DataTable, TablePagination, PageBreadcrumb, PageHeader } from "@presentation/components/common";
import type { Column } from "@/hooks/useColumnReorder";
import { useAppDispatch, useAppSelector } from "@app/store/hooks";
import { fetchColaboradores } from "@app/store/slices/colaboradoresSlice";
import type { Colaborador } from "@domain/entities/Colaborador";
import { useParametros } from "@/hooks/useParametros";
import { logUserAction } from "@shared/utils/firebaseAnalytics";
import { ExportarRelatorioModal } from "@presentation/components/colaboradores";
import { formatDateFromBackend, extractDateOnly } from "@shared/utils/formatUtils";

const Colaboradores = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { colaboradores, status, hasMore } = useAppSelector((state) => state.colaboradores);
  const { token, user } = useAppSelector((state) => state.auth);
  const { getParametro } = useParametros();
  
  const [searchTerm, setSearchTerm] = useState("");
  const [debouncedSearchTerm, setDebouncedSearchTerm] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(20);
  const [totalItems, setTotalItems] = useState(0);

  const orgId = user?.colaboradorOrg?.orgId;
  
  // Obtém labels customizadas ou usa defaults
  const labelColaboradores = getParametro("LABEL_COLABORADORES_TIMESHEET") || "Colaboradores";
  const labelColaborador = getParametro("LABEL_COLABORADOR_TIMESHEET") || "Colaborador(a)"; // Para header da coluna, botões e mensagens
  
  // Verifica se deve exibir o botão de exportar relatório
  // Se for null ou false, retorna false. Se for "true", retorna true
  const exibeRelatorioExtracaoColaborador = (() => {
    const parametro = getParametro("COLABORADOR_RELATORIO_EXTRACAO");
    return parametro !== null && parametro === "true";
  })();

  const [isExportModalOpen, setIsExportModalOpen] = useState(false);

  // Helper para formatar telefone
  const formatPhone = (phone: string): string => {
    if (!phone || phone.length === 0) return '-';
    // Remove caracteres não numéricos
    const cleanPhone = phone.replace(/\D/g, '');
    
    // Celular: (XX) 9XXXX-XXXX (11 dígitos)
    if (cleanPhone.length === 11) {
      return cleanPhone.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
    }
    
    // Telefone fixo: (XX) XXXX-XXXX (10 dígitos)
    if (cleanPhone.length === 10) {
      return cleanPhone.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
    }
    
    // Retorna original se não tiver formato reconhecido
    return phone;
  };

  // Debounce do termo de busca e reset da página
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearchTerm(searchTerm);
      setCurrentPage(1); // Volta para primeira página ao buscar
    }, 500);

    return () => clearTimeout(timer);
  }, [searchTerm]);

  // Fetch colaboradores quando página, busca (debounced) ou limite mudam
  useEffect(() => {
    if (token && orgId && orgId > 0) {
      const cursor = (currentPage - 1) * itemsPerPage;
      void dispatch(fetchColaboradores({ 
        token, 
        orgId, 
        cursor,
        limite: itemsPerPage,
        nomeOuEmail: debouncedSearchTerm
      }));
    }
  }, [dispatch, token, orgId, currentPage, itemsPerPage, debouncedSearchTerm]);

  // Atualiza o total de itens estimado baseado na página atual e se há mais
  useEffect(() => {
    if (colaboradores.length > 0) {
      if (hasMore) {
        // Se há mais, estimamos que há pelo menos mais uma página
        setTotalItems((currentPage * itemsPerPage) + 1);
      } else {
        // Se não há mais, o total é exato
        setTotalItems(((currentPage - 1) * itemsPerPage) + colaboradores.length);
      }
    } else if (currentPage === 1) {
      setTotalItems(0);
    }
  }, [colaboradores, currentPage, itemsPerPage, hasMore]);

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
  };

  const handlePageChange = useCallback((page: number) => {
    setCurrentPage(page);
  }, []);

  const handleItemsPerPageChange = useCallback((items: string) => {
    setItemsPerPage(Number(items));
    setCurrentPage(1);
  }, []);

  const columns: Column[] = [
    { id: "colaborador", label: labelColaborador, sortable: true },
    { id: "situacao", label: "Situação", sortable: true },
    { id: "dataSituacao", label: "Data situação", sortable: true },
    { id: "email", label: "E-mail", sortable: true },
    { id: "telefone", label: "Telefone", sortable: false },
    { id: "acesso", label: "Acesso", sortable: true },
    { id: "acoes", label: "Ações", sortable: false, width: "w-[100px]" },
  ];

  // Transforma os dados para incluir propriedades que correspondem aos IDs das colunas
  // Isso facilita a ordenação automática do DataTable
  const transformedData = useMemo(() => {
    return colaboradores.map(colaborador => ({
      ...colaborador,
      colaborador: colaborador.nome,
      situacao: colaborador.ativo ? 'Ativo' : 'Inativo',
      telefone: colaborador.contatoPrincipal,
      dataSituacao: colaborador.ativo 
        ? extractDateOnly(colaborador.dataAdmissao)
        : extractDateOnly(colaborador.dataInativacao),
      acesso: colaborador.primeiroAcessoRealizado ? 'Concluído' : 'Pendente',
    }));
  }, [colaboradores]);

  const renderCell = (colaborador: Colaborador, columnId: string) => {
    switch (columnId) {
      case "colaborador":
        return (
          <div className="flex items-center gap-3">
            <Avatar className="h-10 w-10">
              <AvatarFallback className="bg-secondary text-secondary-foreground">
                <User className="h-5 w-5" />
              </AvatarFallback>
            </Avatar>
            <span className="font-medium">{colaborador.nome}</span>
          </div>
        );
      case "situacao":
        return (
          <Badge 
            className={
              colaborador.ativo 
                ? "bg-green-100 text-green-700 hover:bg-green-100" 
                : "bg-red-100 text-red-700 hover:bg-red-100"
            }
          >
            {colaborador.ativo ? 'Ativo' : 'Inativo'}
          </Badge>
        );
      case "dataSituacao":
        return (
          <span className="text-sm text-muted-foreground">
            {colaborador.ativo 
              ? formatDateFromBackend(colaborador.dataAdmissao) 
              : formatDateFromBackend(colaborador.dataInativacao)}
          </span>
        );
      case "email":
        return <span className="text-sm">{colaborador.email}</span>;
      case "telefone":
        return <span className="text-sm text-muted-foreground font-mono">{formatPhone(colaborador.contatoPrincipal)}</span>;
      case "acesso":
        return (
          <Badge 
            className={
              colaborador.primeiroAcessoRealizado 
                ? "bg-blue-100 text-blue-700 hover:bg-blue-100" 
                : "bg-yellow-100 text-yellow-700 hover:bg-yellow-100"
            }
          >
            {colaborador.primeiroAcessoRealizado ? 'Concluído' : 'Pendente'}
          </Badge>
        );
      case "acoes":
        return (
          <Button 
            variant="ghost" 
            size="icon"
            className="h-8 w-8"
            title={`Editar ${labelColaborador}`}
            onClick={() => {
              const colaboradorId = colaborador.codColaborador
              // Registra ação do usuário no Firebase Analytics
              logUserAction('GestaoColaboradores', 'FormularioEditarColaborador', {
                colaboradorId: colaboradorId
              }, user)
              // Navega para a página de edição
              navigate(`/colaboradores/editar/${colaboradorId}`)
            }}
          >
            <Edit className="h-4 w-4" />
          </Button>
        );
      default:
        return null;
    }
  };

  return (
    <div className="container mx-auto p-4 space-y-4">
      <PageBreadcrumb 
        items={[
          { label: labelColaboradores }
        ]} 
      />
      
      <PageHeader 
        title={labelColaboradores}
        description={`Gerencie e visualize todos os ${labelColaboradores.toLowerCase()}`}
        actions={
          exibeRelatorioExtracaoColaborador ? (
            <Button 
              variant="outline" 
              className="gap-2"
              onClick={() => setIsExportModalOpen(true)}
            >
              <Download className="h-4 w-4" />
              Exportar Relatório
            </Button>
          ) : undefined
        }
      />

      {/* Search and Add Button */}
      <Card className="rounded-xl">
        <CardContent className="p-6">
          <div className="flex items-center justify-between gap-4">
            <div className="relative flex-1 max-w-md">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Buscar por nome ou email..."
                value={searchTerm}
                onChange={handleSearchChange}
                className="pl-10"
              />
            </div>

            <Button 
              className="gap-2" 
              onClick={() => {
                // Registra ação do usuário no Firebase Analytics
                logUserAction('GestaoColaboradores', 'FormularioNovoColaborador', {}, user)
                // Navega para a página de novo colaborador
                navigate('/colaboradores/novo')
              }}
            >
              <Plus className="h-4 w-4" />
              Adicionar {labelColaborador}
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Colaboradores Table */}
      <Card className="rounded-xl">
        <CardContent className="p-6">
          {status === 'loading' ? (
            <div className="text-center py-8 text-muted-foreground">Carregando {labelColaboradores.toLowerCase()}...</div>
          ) : status === 'failed' ? (
            <div className="text-center py-8 text-destructive">Erro ao carregar {labelColaboradores.toLowerCase()}</div>
          ) : (
            <>
              <DataTable
                columns={columns}
                data={transformedData}
                keyExtractor={(item) => item.codColaborador}
                renderCell={renderCell}
                emptyMessage={`Nenhum ${labelColaborador} encontrado`}
              />
              {totalItems > 0 && (
                <TablePagination
                  currentPage={currentPage}
                  totalItems={totalItems}
                  itemsPerPage={itemsPerPage}
                  onPageChange={handlePageChange}
                  onItemsPerPageChange={handleItemsPerPageChange}
                />
              )}
            </>
          )}
        </CardContent>
      </Card>

      {/* Modal de Exportar Relatório */}
      {exibeRelatorioExtracaoColaborador && (
        <ExportarRelatorioModal
          open={isExportModalOpen}
          onOpenChange={setIsExportModalOpen}
        />
      )}
    </div>
  );
};

export default Colaboradores;
