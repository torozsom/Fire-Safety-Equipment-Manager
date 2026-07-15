import { Component } from '@angular/core';
import { DashboardPageComponent } from './features/dashboard/dashboard-page/dashboard-page.component';

/**
 * Root Angular component that hosts the current dashboard experience.
 *
 * This standalone shell imports the dashboard page directly until the application grows
 * enough to require routed feature pages.
 */
@Component({
  selector: 'app-root',
  imports: [DashboardPageComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {}
