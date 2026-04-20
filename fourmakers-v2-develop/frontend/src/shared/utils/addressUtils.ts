export interface AddressFieldConfig {
  label: string;
  placeholder: string;
}

export interface AddressConfig {
  postalCode: AddressFieldConfig;
  state: AddressFieldConfig;
  city: AddressFieldConfig;
  neighborhood: AddressFieldConfig | null;
  street: AddressFieldConfig;
  number: AddressFieldConfig;
  complement: AddressFieldConfig;
  phone: AddressFieldConfig & { prefix: string };
}

/**
 * Obtém a configuração de campos de endereço baseado no país selecionado
 * @param pais País selecionado (brasil, eua, portugal, etc)
 * @returns Configuração dos campos de endereço com labels e placeholders traduzidos
 */
export const getAddressConfig = (pais: string): AddressConfig => {
  switch (pais) {
    case "brasil":
      return {
        postalCode: { label: "CEP", placeholder: "00000-000" },
        state: { label: "Estado", placeholder: "UF" },
        city: { label: "Cidade", placeholder: "Cidade" },
        neighborhood: { label: "Bairro", placeholder: "Bairro" },
        street: { label: "Rua/Avenida", placeholder: "Nome da rua" },
        number: { label: "Número", placeholder: "123" },
        complement: { label: "Complemento", placeholder: "Apto, bloco, etc" },
        phone: { label: "Telefone", placeholder: "(00) 00000-0000", prefix: "+55" },
      };
    case "eua":
      return {
        postalCode: { label: "ZIP Code", placeholder: "ZIP Code" },
        state: { label: "State", placeholder: "State" },
        city: { label: "City", placeholder: "City" },
        neighborhood: null, // Estados Unidos não usa bairro
        street: { label: "Street/Address", placeholder: "Street name" },
        number: { label: "Number", placeholder: "123" },
        complement: { label: "Complement", placeholder: "Apt, suite, etc" },
        phone: { label: "Phone", placeholder: "000 000 0000", prefix: "+1" },
      };
    case "portugal":
      return {
        postalCode: { label: "Código Postal", placeholder: "Código postal" },
        state: { label: "Distrito", placeholder: "Distrito" },
        city: { label: "Cidade", placeholder: "Cidade" },
        neighborhood: { label: "Freguesia", placeholder: "Freguesia" },
        street: { label: "Rua/Avenida", placeholder: "Nome da rua" },
        number: { label: "Número", placeholder: "123" },
        complement: { label: "Complemento", placeholder: "Complemento" },
        phone: { label: "Telefone", placeholder: "000 000 000", prefix: "+351" },
      };
    default:
      return {
        postalCode: { label: "Código Postal", placeholder: "Código postal" },
        state: { label: "Estado/Região", placeholder: "Estado" },
        city: { label: "Cidade", placeholder: "Cidade" },
        neighborhood: { label: "Bairro/Distrito", placeholder: "Bairro" },
        street: { label: "Rua/Avenida", placeholder: "Nome da rua" },
        number: { label: "Número", placeholder: "123" },
        complement: { label: "Complemento", placeholder: "Complemento" },
        phone: { label: "Telefone", placeholder: "000 000 0000", prefix: "" },
      };
  }
};

