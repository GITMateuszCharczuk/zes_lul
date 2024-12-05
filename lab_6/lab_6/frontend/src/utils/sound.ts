export const playNotificationSound = () => {
  try {
    const audioContext = new (window.AudioContext || (window as any).webkitAudioContext)();
    const oscillator = audioContext.createOscillator();
    const gainNode = audioContext.createGain();

    oscillator.connect(gainNode);
    gainNode.connect(audioContext.destination);

    oscillator.type = 'sine';
    oscillator.frequency.value = 800;
    gainNode.gain.value = 0.1;

    oscillator.start();
    setTimeout(() => {
      oscillator.stop();
      audioContext.close();
    }, 200);
  } catch (error) {
    console.error('Error playing notification sound:', error);
  }
}; 