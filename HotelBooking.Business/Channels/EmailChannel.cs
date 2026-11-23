using HotelBooking.Business.DTOs.Notification;
using HotelBooking.Business.Interfaces;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Enums;
using System.Net;
using System.Text;

namespace HotelBooking.Business.Services.Channels
{
    internal class EmailChannel : INotificationChannel
    {
        private readonly IEmailSender _emailSender;

        public EmailChannel(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public string Name => "Email";

        public bool CanDeliverTo(Guest guest) => !string.IsNullOrWhiteSpace(guest.Email);

        public async Task<bool> SendAsync(Guest guest, NotificationMessage message)
        {
            try
            {
                var plain = $"Hola {guest.FullName},\n\n{message.Summary}\n\nGracias por elegirnos.";
                return await _emailSender.SendAsync(guest.Email, message.Subject, BuildHtml(guest, message), plain);
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Plantilla con CSS en línea y tablas (lo único que respetan Gmail y Outlook).
        private const string Template = """
<table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" bgcolor="#eef2f7" style="background:#eef2f7;">
<tr><td align="center" style="padding:24px 12px;">
  <table role="presentation" width="560" cellpadding="0" cellspacing="0" border="0" bgcolor="#ffffff" style="width:560px;max-width:100%;background:#ffffff;border-collapse:collapse;font-family:Segoe UI,Arial,sans-serif;">
    <tr><td align="center" bgcolor="@@COLOR@@" style="background:@@COLOR@@;padding:30px 24px;color:#ffffff;">
      <div style="font-size:12px;letter-spacing:4px;">HOTELBOOKING</div>
      <div style="font-size:26px;font-weight:bold;padding-top:10px;">@@TITLE@@</div>
    </td></tr>
    <tr><td style="padding:28px 32px 8px 32px;color:#334155;font-size:15px;line-height:22px;">
      Hola <b>@@GUEST@@</b>,<br>@@SUMMARY@@
    </td></tr>
    <tr><td style="padding:12px 32px 28px 32px;">
      <table role="presentation" width="100%" cellpadding="0" cellspacing="0" border="0" style="border-collapse:collapse;border:1px solid #e2e8f0;">
@@ROWS@@
      </table>
    </td></tr>
    <tr><td align="center" bgcolor="#f1f5f9" style="background:#f1f5f9;padding:16px;color:#94a3b8;font-size:12px;">
      Gracias por elegirnos &middot; HotelBooking
    </td></tr>
  </table>
</td></tr>
</table>
""";

        private static string BuildHtml(Guest guest, NotificationMessage m)
        {
            var r = m.Reservation;
            var color = m.Type switch
            {
                NotificationType.Confirmation => "#16a34a",
                NotificationType.Modification => "#d97706",
                NotificationType.Cancellation => "#dc2626",
                _ => "#2563eb"
            };

            var rows = new StringBuilder();
            AddRow(rows, "Reserva", "#" + r.Id, color, false);
            AddRow(rows, "Habitación", r.Room?.Number ?? r.RoomId.ToString(), color, false);
            AddRow(rows, "Entrada", r.CheckIn.ToString("dd/MM/yyyy"), color, false);
            AddRow(rows, "Salida", r.CheckOut.ToString("dd/MM/yyyy"), color, false);
            AddRow(rows, "Total", r.Total.ToString("N2"), color, true);

            return Template
                .Replace("@@COLOR@@", color)
                .Replace("@@TITLE@@", WebUtility.HtmlEncode(m.Subject))
                .Replace("@@GUEST@@", WebUtility.HtmlEncode(guest.FullName))
                .Replace("@@SUMMARY@@", WebUtility.HtmlEncode(m.Summary))
                .Replace("@@ROWS@@", rows.ToString());
        }

        private static void AddRow(StringBuilder sb, string label, string value, string color, bool highlight)
        {
            var bg = highlight ? color : "#ffffff";
            var labelColor = highlight ? "#ffffff" : "#64748b";
            var valueColor = highlight ? "#ffffff" : "#0f172a";
            var size = highlight ? "20px" : "15px";

            sb.Append($"<tr><td bgcolor=\"{bg}\" style=\"background:{bg};padding:12px 16px;border-bottom:1px solid #e2e8f0;color:{labelColor};font-size:12px;text-transform:uppercase;letter-spacing:1px;\">{label}</td>");
            sb.Append($"<td align=\"right\" bgcolor=\"{bg}\" style=\"background:{bg};padding:12px 16px;border-bottom:1px solid #e2e8f0;color:{valueColor};font-size:{size};font-weight:bold;\">{WebUtility.HtmlEncode(value)}</td></tr>\n");
        }
    }
}
