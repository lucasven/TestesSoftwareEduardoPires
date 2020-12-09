using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace NerdStore.Vendas.Domain
{
    public class Voucher
    {
        public string Codigo { get; set; }
        public decimal? PercentualDesconto { get; set; }
        public decimal? ValorDesconto { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataValidade { get; set; }
        public bool Ativo { get; set; }
        public bool Utilizado { get; set; }
        public TipoDescontoVoucher TipoDescontoVoucher { get; internal set; }

        public Voucher(string codigo, decimal? percentualDesconto, decimal? valorDesconto, int quantidade, TipoDescontoVoucher tipoDescontoVoucher, DateTime dataValidade, bool ativo, bool utilizado)
        {
            Codigo = codigo;
            PercentualDesconto = percentualDesconto;
            ValorDesconto = valorDesconto;
            Quantidade = quantidade;
            TipoDescontoVoucher = tipoDescontoVoucher;
            DataValidade = dataValidade;
            Ativo = ativo;
            Utilizado = utilizado;
        }

        public ValidationResult ValidarSeAplicavel()
        {
            return new VoucherAplicavelValidation().Validate(this);
        }
    }

    public class VoucherAplicavelValidation : AbstractValidator<Voucher>
    {
        public static string CodigoErroMsg => "Voucher sem código válido";
        public static string DataValidadeErroMsg => "Este voucher está expirado";
        public static string AtivoErroMsg => "Este voucher não é mais válido";
        public static string UtilizadoErroMsg => "Este voucher já foi utilizado";
        public static string QuantidadeErroMsg => "Este voucher não está mais disponivel";
        public static string ValorDescontoErroMsg => "O valor do desconto precisa ser superior a 0";
        public static string PercentualDescontoErroMsg => "O valor da porcentagem de desconto precisa ser superior";

        public VoucherAplicavelValidation()
        {
            RuleFor(c => c.Codigo).NotEmpty().WithMessage(CodigoErroMsg);

            RuleFor(c => c.DataValidade).Must(DataVencimentoSuperiorAtual).WithMessage(DataValidadeErroMsg);

            RuleFor(c => c.Ativo).Equal(true).WithMessage(AtivoErroMsg);

            RuleFor(c => c.Utilizado).Equal(false).WithMessage(UtilizadoErroMsg);

            RuleFor(c => c.Quantidade).GreaterThan(0).WithMessage(QuantidadeErroMsg);

            When(f => f.TipoDescontoVoucher == TipoDescontoVoucher.Valor, () =>
            {
                RuleFor(c => c.ValorDesconto)
                .NotNull()
                .WithMessage(ValorDescontoErroMsg)
                .GreaterThan(0)
                .WithMessage(ValorDescontoErroMsg);
            });

            When(f => f.TipoDescontoVoucher == TipoDescontoVoucher.Porcentagem, () =>
            {
                RuleFor(c => c.PercentualDesconto)
                .NotNull()
                .WithMessage(PercentualDescontoErroMsg)
                .GreaterThan(0)
                .WithMessage(PercentualDescontoErroMsg);
            });
        }

        protected static bool DataVencimentoSuperiorAtual(DateTime dataValidade)
        {
            return dataValidade >= DateTime.Now;
        }
    }

    public enum TipoDescontoVoucher
    {
        Porcentagem = 0,
        Valor = 1
    }
}
