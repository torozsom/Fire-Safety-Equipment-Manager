import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';

/**
 * Starts the standalone Angular application with the root component and shared providers.
 */
bootstrapApplication(App, appConfig).catch((err) => console.error(err));
