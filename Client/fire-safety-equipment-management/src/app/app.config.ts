import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';

/**
 * Root application configuration for the standalone Angular bootstrap.
 *
 * Registers browser-level error handling, HttpClient, and the router providers used by
 * every feature loaded under the application shell.
 */
export const appConfig: ApplicationConfig = {
  providers: [provideBrowserGlobalErrorListeners(), provideHttpClient(), provideRouter(routes)],
};
