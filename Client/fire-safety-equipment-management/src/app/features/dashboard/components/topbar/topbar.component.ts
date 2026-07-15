import { Component } from '@angular/core';
import { IconComponent } from '../../../../shared/icon/icon.component';

/**
 * Top toolbar for search, notifications, and account-level dashboard actions.
 */
@Component({
  selector: 'app-topbar',
  imports: [IconComponent],
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss',
})
export class TopbarComponent {}
