import React from 'react';
import { Provider as PaperProvider } from 'react-native-paper';
import { NavigationContainer } from '@react-navigation/native';
import { AppNavigator } from './src/navigation/AppNavigator';
import { observer } from 'mobx-react-lite';
import { rootStore } from './src/store/RootStore';
import { StoreProvider } from './src/hooks/useStore';

const AppContent = observer(() => {
  const { authViewModel } = rootStore;

  if (!authViewModel.isInitialized) {
    return null; // Or a loading screen
  }

  return (
    <NavigationContainer>
      <AppNavigator />
    </NavigationContainer>
  );
});

export default function App() {
  return (
    <StoreProvider value={rootStore}>
      <PaperProvider>
        <AppContent />
      </PaperProvider>
    </StoreProvider>
  );
} 