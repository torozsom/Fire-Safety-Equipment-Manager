import { Component, computed, inject, signal } from '@angular/core';
import { DashboardApiService } from '../../../core/services/dashboard-api.service';
import { DASHBOARD_MOCK } from '../../../core/data/dashboard.mock';
import { DashboardData } from '../../../core/models/dashboard.models';
import { ActivityFeedComponent } from '../components/activity-feed/activity-feed.component';
import { DashboardHeaderComponent } from '../components/dashboard-header/dashboard-header.component';
import { ExpiredEquipmentTableComponent } from '../components/expired-equipment-table/expired-equipment-table.component';
import { ExpiringEquipmentListComponent } from '../components/expiring-equipment-list/expiring-equipment-list.component';
import { MetricsGridComponent } from '../components/metrics-grid/metrics-grid.component';
import { QuickActionsComponent } from '../components/quick-actions/quick-actions.component';
import { SidebarComponent } from '../components/sidebar/sidebar.component';
import { StatusBreakdownComponent } from '../components/status-breakdown/status-breakdown.component';
import { TopbarComponent } from '../components/topbar/topbar.component';

/**
 * Dashboard feature container.
 *
 * Owns the dashboard data signal, loads API-backed data through the service, and passes
 * the resulting view model into the presentational dashboard widgets.
 */
@Component({
  selector: 'app-dashboard-page',
  imports: [
    ActivityFeedComponent,
    DashboardHeaderComponent,
    ExpiredEquipmentTableComponent,
    ExpiringEquipmentListComponent,
    MetricsGridComponent,
    QuickActionsComponent,
    SidebarComponent,
    StatusBreakdownComponent,
    TopbarComponent,
  ],
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.scss',
})
export class DashboardPageComponent {
  /**
   * Current dashboard data rendered by the page.
   *
   * Initialized with mock data so the template has a complete view model before the API
   * request resolves.
   */
  readonly dashboard = signal<DashboardData>(DASHBOARD_MOCK);

  /**
   * Total equipment count derived from the status summary buckets.
   */
  readonly totalEquipment = computed(() =>
    this.dashboard().statusSummary.reduce((total, item) => total + item.value, 0),
  );

  /** Service responsible for loading the dashboard view model. */
  private readonly dashboardApi = inject(DashboardApiService);

  /**
   * Starts the dashboard data load when the page component is created.
   */
  constructor() {
    this.dashboardApi.getDashboard().subscribe((dashboard) => this.dashboard.set(dashboard));
  }
}
