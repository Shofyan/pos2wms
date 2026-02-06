import type { ReactNode } from 'react';
import { type LucideIcon, TrendingUp, TrendingDown } from 'lucide-react';
import './StatCard.css';

interface StatCardProps {
    icon: LucideIcon;
    value: string | number;
    label: string;
    trend?: {
        value: number;
        isPositive: boolean;
    };
    color?: 'primary' | 'success' | 'warning' | 'error' | 'info' | 'cyan';
    className?: string;
    children?: ReactNode;
}

const colorMap = {
    primary: {
        bg: 'rgba(99, 102, 241, 0.15)',
        color: '#818cf8',
        glow: 'rgba(99, 102, 241, 0.25)',
    },
    success: {
        bg: 'rgba(34, 197, 94, 0.15)',
        color: '#4ade80',
        glow: 'rgba(34, 197, 94, 0.25)',
    },
    warning: {
        bg: 'rgba(245, 158, 11, 0.15)',
        color: '#fbbf24',
        glow: 'rgba(245, 158, 11, 0.25)',
    },
    error: {
        bg: 'rgba(239, 68, 68, 0.15)',
        color: '#f87171',
        glow: 'rgba(239, 68, 68, 0.25)',
    },
    info: {
        bg: 'rgba(59, 130, 246, 0.15)',
        color: '#60a5fa',
        glow: 'rgba(59, 130, 246, 0.25)',
    },
    cyan: {
        bg: 'rgba(34, 211, 238, 0.15)',
        color: '#22d3ee',
        glow: 'rgba(34, 211, 238, 0.25)',
    },
};

export default function StatCard({
    icon: Icon,
    value,
    label,
    trend,
    color = 'primary',
    className = '',
}: StatCardProps) {
    const colors = colorMap[color];

    return (
        <div
            className={`stat-card ${className}`}
            style={{ '--stat-color': colors.glow } as React.CSSProperties}
        >
            <div
                className="stat-icon"
                style={{ background: colors.bg, color: colors.color }}
            >
                <Icon size={24} />
            </div>
            <div className="stat-value">{value}</div>
            <div className="stat-label">{label}</div>
            {trend && (
                <div className={`stat-trend ${trend.isPositive ? 'up' : 'down'}`}>
                    {trend.isPositive ? (
                        <TrendingUp size={14} />
                    ) : (
                        <TrendingDown size={14} />
                    )}
                    <span>{Math.abs(trend.value)}%</span>
                </div>
            )}
        </div>
    );
}
