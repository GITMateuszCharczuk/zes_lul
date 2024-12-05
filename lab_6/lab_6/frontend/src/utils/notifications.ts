export const showTicketUpdateNotification = (ticket: { title: string }) => {
  alert(`Zgłoszenie o tytule "${ticket.title}" zostało zmodyfikowane przez administratora`);
}; 