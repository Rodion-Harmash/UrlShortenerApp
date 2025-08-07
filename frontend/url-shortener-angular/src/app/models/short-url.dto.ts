export interface ShortUrlDto {
  id: number;
  originalUrl: string;
  shortCode: string;
  createdAt: string;
  createdByDisplayName: string;
  createdById: string;
  clickCount: number;
}
