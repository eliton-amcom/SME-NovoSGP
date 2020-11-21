﻿using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SGP.Aplicacao
{
    public class ObterUsuarioIdPorRfOuCriaQuery : IRequest<long>
    {
        public ObterUsuarioIdPorRfOuCriaQuery(string usuarioRf)
        {
            UsuarioRf = usuarioRf;
        }

        public string UsuarioRf { get; set; }
    }

    public class ObterUsuarioIdPorRfOuCriaQueryValidator : AbstractValidator<ObterUsuarioIdPorRfOuCriaQuery>
    {
        public ObterUsuarioIdPorRfOuCriaQueryValidator()
        {
            RuleFor(c => c.UsuarioRf)
               .NotEmpty()
               .WithMessage("O RF do usuário deve ser informado para consulta.");
        }
    }
}
