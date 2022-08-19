using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditCard.Core
{
    public enum DecisaoAprovacao
    {
        Desconhecido,           //desconhecido
        AceitacaoAutomatica,    //aceito automaticamente pelo sistema
        RecusaAutomatica,       //recusado automaticamente pelo sistema
        DecisaoManual           //precisa de uma revisão pessoal
    }
}
