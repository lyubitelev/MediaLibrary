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

  public GerAllMediaAsync(): Observable<MediaInfoDto[]> {
    return this.http.get<MediaInfoDto[]>(`${Settings.apiUrl}/MediaFile/GetAllVideoFiles`);
  }
}
