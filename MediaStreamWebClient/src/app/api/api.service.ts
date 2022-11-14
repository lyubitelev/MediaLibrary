import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MediaInfoDto } from '../models/media-info';
import Settings from '../../assets/settings.json';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private http: HttpClient) { }

  public GetAllMediaAsync(fileName?: string): Observable<MediaInfoDto[]> {
    const searchApiPath = fileName
      ? `${Settings.apiUrl}/MediaFile/GetAllVideoFileInfos/${this.encodeURIComponent(fileName!)}`
      : `${Settings.apiUrl}/MediaFile/GetAllVideoFileInfos/`;

    return this.http.get<MediaInfoDto[]>(searchApiPath);
  }

  public GetMediaStreamUrl(fileName: string): string {
    return `${Settings.apiUrl}/MediaStream/GetMediaStream/${this.encodeURIComponent(fileName)}`;
  }

  private encodeURIComponent(stringValue: string) {
    return encodeURIComponent(stringValue).replace(/[!'()*]/g, function (c) {
      return `%${c.charCodeAt(0).toString(16)}`;
    });
  }
}
