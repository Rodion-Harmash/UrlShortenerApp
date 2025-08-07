import { Component, OnInit, OnDestroy } from '@angular/core';
import { UrlService } from '../../services/url.service';
import { ShortUrlDto } from '../../models/short-url.dto';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-url-table',
  templateUrl: './url-table.component.html',
  styleUrls: ['./url-table.component.css']
})
export class UrlTableComponent implements OnInit, OnDestroy {
  urls: ShortUrlDto[] = [];
  newUrl: string = '';
  error: string | null = null;
  copyStatus: { [id: number]: boolean } = {};

  isAuthenticated: boolean = false;
  isAdmin: boolean = false;
  currentDisplayName: string = '';

  private intervalId: any;
  private authSub?: Subscription;

  sortColumn: keyof ShortUrlDto = 'createdAt';
  sortDirection: 'asc' | 'desc' = 'desc';

  constructor(
    private urlService: UrlService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authSub = this.authService.isLoggedIn$.subscribe(() => {
      const user = this.authService.getUserInfo();
      this.isAuthenticated = !!user;
      this.isAdmin = user?.role === 'Admin';
      this.currentDisplayName = user?.displayName ?? '';
    });

    this.loadUrls();

    this.intervalId = setInterval(() => {
      this.loadUrls();
    }, 5000);
  }

  ngOnDestroy(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
    this.authSub?.unsubscribe();
  }

  loadUrls(): void {
    this.urlService.getAll().subscribe({
      next: urls => {
        this.urls = this.sortUrls(urls);
      },
      error: () => {
        this.error = 'Failed to load URLs.';
      }
    });
  }

  addUrl(): void {
    this.error = null;

    this.urlService.create(this.newUrl).subscribe({
      next: (url) => {
        this.urls.push(url);
        this.urls = this.sortUrls(this.urls);
        this.newUrl = '';
      },
      error: (err) => {
        this.error = typeof err.error === 'string' ? err.error : 'Failed to shorten the URL.';
      }
    });
  }

  deleteUrl(id: number): void {
    this.urlService.delete(id).subscribe({
      next: () => {
        this.urls = this.urls.filter(url => url.id !== id);
      },
      error: () => {
        this.error = 'Failed to delete the URL.';
      }
    });
  }

  copyToClipboard(id: number, shortCode: string): void {
    const link = `http://localhost:5020/s/${shortCode}`;
    navigator.clipboard.writeText(link).then(() => {
      this.copyStatus[id] = true;
      setTimeout(() => {
        this.copyStatus[id] = false;
      }, 1500);
    });
  }

  viewDetails(id: number): void {
    this.router.navigate(['/urls', id]);
  }

  sortBy(column: keyof ShortUrlDto): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = column === 'createdAt' ? 'desc' : 'asc';
    }

    this.urls = this.sortUrls(this.urls);
  }

  private sortUrls(urls: ShortUrlDto[]): ShortUrlDto[] {
    const sorted = [...urls];
    sorted.sort((a, b) => {
      let aValue = a[this.sortColumn];
      let bValue = b[this.sortColumn];

      if (this.sortColumn === 'createdAt') {
        aValue = new Date(aValue as string).getTime();
        bValue = new Date(bValue as string).getTime();
      }

      if (typeof aValue === 'string') aValue = aValue.toLowerCase();
      if (typeof bValue === 'string') bValue = bValue.toLowerCase();

      if (aValue < bValue) return this.sortDirection === 'asc' ? -1 : 1;
      if (aValue > bValue) return this.sortDirection === 'asc' ? 1 : -1;
      return 0;
    });

    return sorted;
  }
}
