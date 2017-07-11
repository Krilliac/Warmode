using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class NewSessionTicket
	{
		protected readonly long mTicketLifetimeHint;

		protected readonly byte[] mTicket;

		public virtual long TicketLifetimeHint
		{
			get
			{
				return this.mTicketLifetimeHint;
			}
		}

		public virtual byte[] Ticket
		{
			get
			{
				return this.mTicket;
			}
		}

		public NewSessionTicket(long ticketLifetimeHint, byte[] ticket)
		{
			this.mTicketLifetimeHint = ticketLifetimeHint;
			this.mTicket = ticket;
		}

		public virtual void Encode(Stream output)
		{
			TlsUtilities.WriteUint32(this.mTicketLifetimeHint, output);
			TlsUtilities.WriteOpaque16(this.mTicket, output);
		}

		public static NewSessionTicket Parse(Stream input)
		{
			long ticketLifetimeHint = TlsUtilities.ReadUint32(input);
			byte[] ticket = TlsUtilities.ReadOpaque16(input);
			return new NewSessionTicket(ticketLifetimeHint, ticket);
		}
	}
}
