import { Component } from '@angular/core';
import { IconComponent, IconName } from '../../../../shared/icon/icon.component';

/**
 * Navigation entry rendered by the dashboard sidebar.
 */
interface NavItem {
  /** Localized navigation label. */
  readonly label: string;

  /** Icon displayed before the navigation label. */
  readonly icon: IconName;
}

/**
 * Sidebar navigation for the fire safety equipment management dashboard.
 */
@Component({
  selector: 'app-sidebar',
  imports: [IconComponent],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  /** Ordered navigation items available from the dashboard shell. */
  readonly navItems: NavItem[] = [
    { label: 'Dashboard', icon: 'home' },
    { label: 'Eszközök', icon: 'box' },
    { label: 'Telephelyek', icon: 'building' },
    { label: 'Karbantartások', icon: 'calendar' },
    { label: 'Hibabejelentések', icon: 'alert' },
    { label: 'Munkalapok', icon: 'clipboard' },
    { label: 'Értesítések', icon: 'bell' },
    { label: 'Dokumentumok', icon: 'file' },
    { label: 'Riportok', icon: 'chart' },
    { label: 'Ügyfelek', icon: 'users' },
    { label: 'Adminisztráció', icon: 'settings' },
  ];
}
