import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import * as z from "zod";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Checkbox } from "@/components/ui/checkbox";
import { toast } from "sonner";
import { Calendar } from "@/components/ui/calendar";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { CalendarIcon, Save, Camera, ChevronDown } from "@/components/ui/system-icons";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import { cn } from "@/lib/utils";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";
import { useState } from "react";

const formSchema = z.object({
  // Perfil
  fullName: z.string().min(3, "Nome completo deve ter no mínimo 3 caracteres"),
  birthDate: z.date({ required_error: "Data de nascimento é obrigatória" }),
  cpf: z.string().regex(/^\d{3}\.\d{3}\.\d{3}-\d{2}$/, "CPF inválido (formato: 000.000.000-00)"),
  rg: z.string().min(5, "RG deve ter no mínimo 5 caracteres"),
  maritalStatus: z.string({ required_error: "Estado civil é obrigatório" }),
  education: z.string({ required_error: "Escolaridade é obrigatória" }),
  ethnicity: z.string({ required_error: "Etnia é obrigatória" }),
  gender: z.string({ required_error: "Gênero é obrigatório" }),
  isRefugee: z.boolean().default(false),
  
  // Endereço
  zipCode: z.string().regex(/^\d{5}-\d{3}$/, "CEP inválido (formato: 00000-000)"),
  street: z.string().min(3, "Logradouro é obrigatório"),
  number: z.string().min(1, "Número é obrigatório"),
  complement: z.string().optional(),
  neighborhood: z.string().min(2, "Bairro é obrigatório"),
  city: z.string().min(2, "Cidade é obrigatória"),
  state: z.string().length(2, "UF deve ter 2 caracteres"),
  
  // Contato
  professionalEmail: z.string().email("Email profissional inválido"),
  alternativeEmail: z.string().email("Email alternativo inválido").optional().or(z.literal("")),
  phone: z.string().regex(/^\(\d{2}\) \d{5}-\d{4}$/, "Celular inválido (formato: (00) 00000-0000)"),
});

type FormData = z.infer<typeof formSchema>;

const PersonalData = () => {
  const [perfilOpen, setPerfilOpen] = useState(true);
  const [enderecoOpen, setEnderecoOpen] = useState(true);
  const [contatoOpen, setContatoOpen] = useState(true);
  
  const form = useForm<FormData>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      isRefugee: false,
    },
  });

  const onSubmit = (data: FormData) => {
    console.log(data);
    toast.success("Dados salvos com sucesso!");
  };

  return (
    <div className="container max-w-5xl mx-auto py-8 px-4">
      <div className="mb-8">
        <h1 className="page-title mb-2">Dados Pessoais</h1>
        <p className="text-muted-foreground">
          Mantenha suas informações pessoais atualizadas
        </p>
      </div>

      {/* Profile Photo and Name Section */}
      <div className="mb-8 flex items-center gap-6">
        <div className="relative">
          <Avatar className="h-24 w-24">
            <AvatarImage src="https://github.com/shadcn.png" alt="Foto de perfil" />
            <AvatarFallback className="text-2xl">JD</AvatarFallback>
          </Avatar>
          <Button
            size="icon"
            variant="outline"
            className="absolute -bottom-2 -right-2 h-10 w-10 rounded-full bg-background shadow-md"
            type="button"
          >
            <Camera className="h-4 w-4" />
          </Button>
        </div>
        <div>
          <h2 className="text-2xl font-semibold text-foreground">João da Silva</h2>
          <p className="text-sm text-muted-foreground">Atualizar foto de perfil</p>
        </div>
      </div>

      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          {/* Seção Perfil */}
          <Card>
            <Collapsible open={perfilOpen} onOpenChange={setPerfilOpen}>
              <CardHeader className="cursor-pointer" onClick={() => setPerfilOpen(!perfilOpen)}>
                <CollapsibleTrigger className="flex items-center justify-between w-full [&[data-state=open]>.material-symbols-outlined]:rotate-180">
                  <CardTitle>Perfil</CardTitle>
                  <ChevronDown className="h-5 w-5 transition-transform duration-200" />
                </CollapsibleTrigger>
              </CardHeader>
              <CollapsibleContent>
                <CardContent className="grid gap-6">
              <div className="grid md:grid-cols-2 gap-6">
                <FormField
                  control={form.control}
                  name="fullName"
                  render={({ field }) => (
                    <FormItem className="md:col-span-2">
                      <FormLabel>Nome Completo</FormLabel>
                      <FormControl>
                        <Input placeholder="Digite seu nome completo" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="birthDate"
                  render={({ field }) => (
                    <FormItem className="flex flex-col">
                      <FormLabel>Data de Nascimento</FormLabel>
                      <Popover>
                        <PopoverTrigger asChild>
                          <FormControl>
                            <Button
                              variant="outline"
                              className={cn(
                                "w-full pl-3 text-left font-normal",
                                !field.value && "text-muted-foreground"
                              )}
                            >
                              {field.value ? (
                                format(field.value, "dd/MM/yyyy", { locale: ptBR })
                              ) : (
                                <span>Selecione a data</span>
                              )}
                              <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                            </Button>
                          </FormControl>
                        </PopoverTrigger>
                        <PopoverContent className="w-auto p-0" align="start">
                          <Calendar
                            mode="single"
                            selected={field.value}
                            onSelect={field.onChange}
                            disabled={(date) =>
                              date > new Date() || date < new Date("1900-01-01")
                            }
                            initialFocus
                            className="pointer-events-auto"
                          />
                        </PopoverContent>
                      </Popover>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="cpf"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>CPF</FormLabel>
                      <FormControl>
                        <Input placeholder="000.000.000-00" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="rg"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>RG</FormLabel>
                      <FormControl>
                        <Input placeholder="Digite seu RG" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="maritalStatus"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Estado Civil</FormLabel>
                      <Select onValueChange={field.onChange} defaultValue={field.value}>
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          <SelectItem value="solteiro">Solteiro(a)</SelectItem>
                          <SelectItem value="casado">Casado(a)</SelectItem>
                          <SelectItem value="divorciado">Divorciado(a)</SelectItem>
                          <SelectItem value="viuvo">Viúvo(a)</SelectItem>
                          <SelectItem value="uniao-estavel">União Estável</SelectItem>
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="education"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Escolaridade</FormLabel>
                      <Select onValueChange={field.onChange} defaultValue={field.value}>
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          <SelectItem value="fundamental-incompleto">Fundamental Incompleto</SelectItem>
                          <SelectItem value="fundamental-completo">Fundamental Completo</SelectItem>
                          <SelectItem value="medio-incompleto">Médio Incompleto</SelectItem>
                          <SelectItem value="medio-completo">Médio Completo</SelectItem>
                          <SelectItem value="superior-incompleto">Superior Incompleto</SelectItem>
                          <SelectItem value="superior-completo">Superior Completo</SelectItem>
                          <SelectItem value="pos-graduacao">Pós-graduação</SelectItem>
                          <SelectItem value="mestrado">Mestrado</SelectItem>
                          <SelectItem value="doutorado">Doutorado</SelectItem>
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="ethnicity"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Etnia</FormLabel>
                      <Select onValueChange={field.onChange} defaultValue={field.value}>
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          <SelectItem value="branca">Branca</SelectItem>
                          <SelectItem value="preta">Preta</SelectItem>
                          <SelectItem value="parda">Parda</SelectItem>
                          <SelectItem value="amarela">Amarela</SelectItem>
                          <SelectItem value="indigena">Indígena</SelectItem>
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="gender"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Gênero</FormLabel>
                      <Select onValueChange={field.onChange} defaultValue={field.value}>
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Selecione" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          <SelectItem value="masculino">Masculino</SelectItem>
                          <SelectItem value="feminino">Feminino</SelectItem>
                          <SelectItem value="nao-binario">Não-binário</SelectItem>
                          <SelectItem value="outro">Outro</SelectItem>
                          <SelectItem value="prefiro-nao-informar">Prefiro não informar</SelectItem>
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="isRefugee"
                  render={({ field }) => (
                    <FormItem className="flex flex-row items-start space-x-3 space-y-0 rounded-md border p-4">
                      <FormControl>
                        <Checkbox
                          checked={field.value}
                          onCheckedChange={field.onChange}
                        />
                      </FormControl>
                      <div className="space-y-1 leading-none">
                        <FormLabel>Pessoa Refugiada</FormLabel>
                      </div>
                    </FormItem>
                  )}
                />
              </div>
            </CardContent>
            </CollapsibleContent>
          </Collapsible>
          </Card>

          {/* Seção Endereço */}
          <Card>
            <Collapsible open={enderecoOpen} onOpenChange={setEnderecoOpen}>
              <CardHeader className="cursor-pointer" onClick={() => setEnderecoOpen(!enderecoOpen)}>
                <CollapsibleTrigger className="flex items-center justify-between w-full [&[data-state=open]>.material-symbols-outlined]:rotate-180">
                  <CardTitle>Endereço</CardTitle>
                  <ChevronDown className="h-5 w-5 transition-transform duration-200" />
                </CollapsibleTrigger>
              </CardHeader>
              <CollapsibleContent>
                <CardContent className="grid gap-6">
              <div className="grid md:grid-cols-3 gap-6">
                <FormField
                  control={form.control}
                  name="zipCode"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>CEP</FormLabel>
                      <FormControl>
                        <Input placeholder="00000-000" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="street"
                  render={({ field }) => (
                    <FormItem className="md:col-span-2">
                      <FormLabel>Logradouro</FormLabel>
                      <FormControl>
                        <Input placeholder="Rua, avenida..." {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="number"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Número</FormLabel>
                      <FormControl>
                        <Input placeholder="123" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="complement"
                  render={({ field }) => (
                    <FormItem className="md:col-span-2">
                      <FormLabel>Complemento (opcional)</FormLabel>
                      <FormControl>
                        <Input placeholder="Apto, bloco..." {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="neighborhood"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Bairro</FormLabel>
                      <FormControl>
                        <Input placeholder="Digite o bairro" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="city"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Cidade</FormLabel>
                      <FormControl>
                        <Input placeholder="Digite a cidade" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="state"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>UF</FormLabel>
                      <FormControl>
                        <Input placeholder="SP" maxLength={2} {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
            </CardContent>
            </CollapsibleContent>
          </Collapsible>
          </Card>

          {/* Seção Contato */}
          <Card>
            <Collapsible open={contatoOpen} onOpenChange={setContatoOpen}>
              <CardHeader className="cursor-pointer" onClick={() => setContatoOpen(!contatoOpen)}>
                <CollapsibleTrigger className="flex items-center justify-between w-full [&[data-state=open]>.material-symbols-outlined]:rotate-180">
                  <CardTitle>Contato</CardTitle>
                  <ChevronDown className="h-5 w-5 transition-transform duration-200" />
                </CollapsibleTrigger>
              </CardHeader>
              <CollapsibleContent>
                <CardContent className="grid gap-6">
              <div className="grid md:grid-cols-2 gap-6">
                <FormField
                  control={form.control}
                  name="professionalEmail"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Email Profissional</FormLabel>
                      <FormControl>
                        <Input type="email" placeholder="seu.email@empresa.com" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="alternativeEmail"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Email Alternativo (opcional)</FormLabel>
                      <FormControl>
                        <Input type="email" placeholder="email.alternativo@exemplo.com" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="phone"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Celular</FormLabel>
                      <FormControl>
                        <Input placeholder="(00) 00000-0000" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
            </CardContent>
            </CollapsibleContent>
          </Collapsible>
          </Card>

          <div className="flex justify-end gap-4">
            <Button type="button" variant="outline">
              Cancelar
            </Button>
            <Button type="submit">
              <Save className="mr-2 h-4 w-4" />
              Salvar Dados
            </Button>
          </div>
        </form>
      </Form>
    </div>
  );
};

export default PersonalData;
